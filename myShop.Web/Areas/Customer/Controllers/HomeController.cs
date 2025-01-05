using Microsoft.AspNetCore.Mvc;
using myShop.Entities.Models;
using myShop.Entities.Repositories;
using myShop.Entities.ViewModels;

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

        public IActionResult Details(int id)
        {
            ProductDetailsCard shippingCardDetails = new ProductDetailsCard()
            {
                Product = unitOfWork.Product.GetFirstOrDefault(p => p.Id == id, includeValue: "Category"),
                Count = 1
            };
            return View(shippingCardDetails);
        }
    }
}
