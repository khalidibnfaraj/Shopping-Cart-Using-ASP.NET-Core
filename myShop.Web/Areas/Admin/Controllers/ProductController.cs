using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using myShop.DataAccess.Data;
using myShop.DataAccess.RepositoriesImplementation;
using myShop.Entities.Models;
using myShop.Entities.Repositories;
using myShop.Entities.ViewModels;
using System.Runtime.ConstrainedExecution;

namespace myShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            this.unitOfWork = unitOfWork;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var products = unitOfWork.Product.GetAll();
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = unitOfWork.Category.GetAll().Select(sli => new SelectListItem()
                {
                    Text = sli.Name,
                    Value = sli.Id.ToString()
                })
            };
            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductVM productVM, IFormFile Imgfile)
        {
            string RootPath = webHostEnvironment.WebRootPath;
            if (ModelState.IsValid)
            {
                if (Imgfile != null)
                {
                    string uniqueFilePath = Guid.NewGuid().ToString();
                    string ImgfileExtension = Path.GetExtension(Imgfile.FileName);
                    string uniqueFileName = uniqueFilePath + ImgfileExtension;
                    string filePath = Path.Combine(RootPath, @"Images\Products");

                    string ImageFileFullPath = Path.Combine(filePath, uniqueFileName);

                    var FileStreamPlace = new FileStream(ImageFileFullPath, FileMode.Create);
                    using (FileStreamPlace)
                    {
                        Imgfile.CopyTo(FileStreamPlace);
                    }

                    productVM.Product.Image = @"Images\Products" + uniqueFileName;
                }

                    unitOfWork.Product.Add(productVM.Product);
                    unitOfWork.Complete();
                    TempData["Create"] = "Product has added successfully";
                    return RedirectToAction("Index");
                }
            
            return View(productVM.Product);
        }

        [HttpGet]
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
            {
                NotFound();
            }
            Product product = unitOfWork.Product.GetFirstOrDefault(c => c.Id == id);
            return View(product);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Product productVM)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Product.Update(productVM);
                unitOfWork.Complete();
                TempData["Update"] = "Product has updated successfully";
                return RedirectToAction("Index");
            }
            return View("Update");
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                NotFound();
            }

            Product product = unitOfWork.Product.GetFirstOrDefault(c => c.Id == id);
            return View(product);
        }
        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            Product product = unitOfWork.Product.GetFirstOrDefault(c => c.Id == id); ;

            if (product == null)
            {
                NotFound();
            }
            unitOfWork.Product.Remove(product);
            unitOfWork.Complete();
            TempData["Delete"] = "Product has deleted successfully";
            return RedirectToAction("Index");
        }
    }
}

