using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myShop.DataAccess.Data;
using myShop.DataAccess.RepositoriesImplementation;
using myShop.Entities.Models;
using myShop.Entities.Repositories;
using myShop.Entities.ViewModels;
using myShop.Utilities;
using NuGet.Configuration;
using System.Security.Claims;

namespace myShop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public ShoppingCartVM shoppingCartVM {  get; set; }
        public decimal TotalCartsPrice {  get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var Products = unitOfWork.ShoppingCart.GetAll(product => product.ApplicationUserId == claim.Value, includeValue: "Product");
            shoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCarts = Products,
            };

            foreach (var product in shoppingCartVM.ShoppingCarts)
            {
                shoppingCartVM.TotalCardsPrice += (product.count)*(product.Product.Price); 
            }
           
            return View(shoppingCartVM);
        }

        public IActionResult Plus(int cartId) {
            ShoppingCart shoppingCartItem = unitOfWork.ShoppingCart
                .GetFirstOrDefault(p=>p.Id == cartId);
            unitOfWork.ShoppingCart.IncreaseCount(shoppingCartItem, 1);
            unitOfWork.Complete();
            return RedirectToAction("Index");
        }
		public IActionResult Minus(int cartId)
		{
			ShoppingCart shoppingCartItem = unitOfWork.ShoppingCart
				.GetFirstOrDefault(p => p.Id == cartId);
            if(shoppingCartItem.count <= 1)
            {
                unitOfWork.ShoppingCart.Remove(shoppingCartItem);
				unitOfWork.Complete();
				return RedirectToAction("Index", "Home");
			}
            else
            {
				unitOfWork.ShoppingCart.DecreaseCount(shoppingCartItem, 1);

			}
			unitOfWork.Complete();
			return RedirectToAction("Index");
		}

        public IActionResult Remove(int cartId)
        {
			ShoppingCart shoppingCartItem = unitOfWork.ShoppingCart
				.GetFirstOrDefault(p => p.Id == cartId);
			unitOfWork.ShoppingCart.Remove(shoppingCartItem);
			unitOfWork.Complete();
            return RedirectToAction("Index");
           

			}


		}

	}
}
