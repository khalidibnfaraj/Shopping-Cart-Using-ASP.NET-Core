using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using myShop.DataAccess.Implementation;
using myShop.Entities.Models;
using myShop.Entities.Repositories;
using myShop.Entities.ViewModels;
using myShop.Utilities;
using Stripe.Checkout;
using System.Security.Claims;

namespace myshop.Web.Areas.Customer.Controllers
{
	[Area("Customer")]
	[Authorize]
	public class CartController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public ShoppingCartVM ShoppingCartVM { get; set; }

		public CartController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			ShoppingCartVM = new ShoppingCartVM()
			{
				ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeValue: "Product"),
				OrderHeader = new()

			};

			foreach (var item in ShoppingCartVM.ShoppingCarts)
			{
				ShoppingCartVM.OrderHeader.TotalPrice += (item.count * item.Product.Price);
			}

			return View(ShoppingCartVM);
		}

		[HttpGet]
		public IActionResult Summary()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			ShoppingCartVM = new ShoppingCartVM()
			{
				ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeValue: "Product"),
				OrderHeader = new()
			};

			ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == claim.Value);

			ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
			ShoppingCartVM.OrderHeader.Address = ShoppingCartVM.OrderHeader.ApplicationUser.Address;
			ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
			ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;

			foreach (var item in ShoppingCartVM.ShoppingCarts)
			{
				ShoppingCartVM.OrderHeader.TotalPrice += (item.count * item.Product.Price);
			}

			return View(ShoppingCartVM);
		}

		[HttpPost]
		[ActionName("Summary")]
		[ValidateAntiForgeryToken]
		public IActionResult POSTSummary(ShoppingCartVM ShoppingCartVM)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			ShoppingCartVM.ShoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeValue: "Product");


			ShoppingCartVM.OrderHeader.OrderStatus = SD.Pending;
			ShoppingCartVM.OrderHeader.PaymentStatus = SD.Pending;
			ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
			ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;


			foreach (var item in ShoppingCartVM.ShoppingCarts)
			{
				ShoppingCartVM.OrderHeader.TotalPrice += (item.count * item.Product.Price);
			}

			_unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
			_unitOfWork.Complete();

			foreach (var item in ShoppingCartVM.ShoppingCarts)
			{
				OrderDetails orderDetail = new OrderDetails()
				{
					ProductId = item.ProductId,
					OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
					Price = item.Product.Price,
					Count = item.count
				};

				_unitOfWork.OrderDetails.Add(orderDetail);
				_unitOfWork.Complete();
			}

			var domain = "http://localhost:24442/";
			var options = new SessionCreateOptions
			{
				LineItems = new List<SessionLineItemOptions>(),

				Mode = "payment",
				SuccessUrl = domain + $"customer/cart/orderconfirmation?id={ShoppingCartVM.OrderHeader.Id}",
				CancelUrl = domain + $"customer/cart/index",
			};

			foreach (var item in ShoppingCartVM.ShoppingCarts)
			{
				var sessionlineoption = new SessionLineItemOptions
				{
					PriceData = new SessionLineItemPriceDataOptions
					{
						UnitAmount = (long)(item.Product.Price * 100),
						Currency = "usd",
						ProductData = new SessionLineItemPriceDataProductDataOptions
						{
							Name = item.Product.Name,
						},
					},
					Quantity = item.count,
				};
				options.LineItems.Add(sessionlineoption);
			}


			var service = new SessionService();
			Session session = service.Create(options);
			ShoppingCartVM.OrderHeader.SessionId = session.Id;

			_unitOfWork.Complete();

			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);

			//_unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.CartsList);
			//         _unitOfWork.Complete();
			//         return RedirectToAction("Index","Home");

		}

		public IActionResult OrderConfirmation(int id)
		{
			OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
			var service = new SessionService();
			Session session = service.Get(orderHeader.SessionId);

			if (session.PaymentStatus.ToLower() == "paid")
			{
				_unitOfWork.OrderHeader.UpdateStatus(id, SD.Approve, SD.Approve);
				orderHeader.PaymentIntentId = session.PaymentIntentId;
				_unitOfWork.Complete();
			}
			List<ShoppingCart> shoppingcarts = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
			HttpContext.Session.Clear();
			_unitOfWork.ShoppingCart.RemoveRange(shoppingcarts);
			_unitOfWork.Complete();
			return View(id);
		}

		public IActionResult Plus(int cartid)
		{
			var shoppingcart = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == cartid);
			_unitOfWork.ShoppingCart.IncreaseCount(shoppingcart, 1);
			_unitOfWork.Complete();
			return RedirectToAction("Index");
		}

		public IActionResult Minus(int cartid)
		{
			var shoppingcart = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == cartid);

			if (shoppingcart.count <= 1)
			{
				_unitOfWork.ShoppingCart.Remove(shoppingcart);
				var count = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == shoppingcart.ApplicationUserId).ToList().Count() - 1;
				HttpContext.Session.SetInt32(SD.SessionKey, count);
			}
			else
			{
				_unitOfWork.ShoppingCart.DecreaseCount(shoppingcart, 1);

			}
			_unitOfWork.Complete();
			return RedirectToAction("Index");
		}

		public IActionResult Remove(int cartid)
		{
			var shoppingcart = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == cartid);
			_unitOfWork.ShoppingCart.Remove(shoppingcart);
			_unitOfWork.Complete();
			var count = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == shoppingcart.ApplicationUserId).ToList().Count();
			HttpContext.Session.SetInt32(SD.SessionKey, count);
			return RedirectToAction("Index");
		}
	}
}
