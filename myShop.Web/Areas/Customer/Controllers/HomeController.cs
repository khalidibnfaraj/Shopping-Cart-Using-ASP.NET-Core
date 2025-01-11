using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myShop.DataAccess.RepositoriesImplementation;
using myShop.Entities.Models;
using myShop.Entities.Repositories;
using myShop.Entities.ViewModels;
using System.Security.Claims;

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
        public IActionResult Index()
        {
            var products = unitOfWork.Product.GetAll();
            return View(products);
        }


        public IActionResult Details(int ProductId)
        {
            ShoppingCart shippingCardDetails = new ShoppingCart()
            {
                ProductId = ProductId,
                Product = unitOfWork.Product.GetFirstOrDefault(p => p.Id == ProductId, includeValue: "Category"),
                count = 1
            };
            return View(shippingCardDetails);
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
            }
            else { 
              unitOfWork.ShoppingCart.UpdateCount(changedCardCount, shoppingCart.count); 
            }

            unitOfWork.Complete();

            return RedirectToAction("Index");
        }
    }
}
