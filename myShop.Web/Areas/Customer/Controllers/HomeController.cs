using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myShop.DataAccess.RepositoriesImplementation;
using myShop.Entities.Models;
using myShop.Entities.Repositories;
using myShop.Entities.ViewModels;
using myShop.Utilities;
using System.Security.Claims;
using X.PagedList;

namespace myShop.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index(int? page)
        {
            var PageNumber = page ?? 1;
            int PageSize = 3;


            var products = unitOfWork.Product.GetAll().ToPagedList(PageNumber, PageSize);
            return View(products);
        }


       
        [HttpGet]
        [Authorize]
        public IActionResult Details(int ProductId)
        {
            ShoppingCart obj = new ShoppingCart()
            {
                ProductId = ProductId,
                Product = unitOfWork.Product.GetFirstOrDefault(p=> p.Id == ProductId, includeValue: "Category"),
                count = 1
            };

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.ApplicationUserId = claim.Value;

            ShoppingCart changedCardCount = unitOfWork.ShoppingCart
                .GetFirstOrDefault(u => u.ApplicationUserId == claim.Value && u.ProductId == shoppingCart.ProductId);

            if (changedCardCount == null)
            {
                unitOfWork.ShoppingCart.Add(shoppingCart);
                unitOfWork.Complete();
                HttpContext.Session.SetInt32(SD.SessionKey,
                    unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value).ToList().Count()
                   );

            }
            else
            {
                unitOfWork.ShoppingCart.IncreaseCount(changedCardCount, shoppingCart.count);
                unitOfWork.Complete();
            }

            return RedirectToAction("Index");
        }
    }
}
