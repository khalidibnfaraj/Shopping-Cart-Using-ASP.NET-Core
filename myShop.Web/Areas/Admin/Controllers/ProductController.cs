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
        [HttpGet]

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetData()
        {
            var products = unitOfWork.Product.GetAll(includeValue: "Category");
            return Json(new { data = products });
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

                    using (var FileStreamPlace = new FileStream(ImageFileFullPath, FileMode.Create))
                    {
                        Imgfile.CopyTo(FileStreamPlace);
                    }

                    productVM.Product.Image = @"Images\Products\" + uniqueFileName;
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
            
            ProductVM productVM = new ProductVM()
            {
                Product = unitOfWork.Product.GetFirstOrDefault(c => c.Id == id),
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
        public IActionResult Update(ProductVM productVM, IFormFile? Imgfile)
        {
            if (ModelState.IsValid)
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

                        if (productVM.Product.Image != null)
                        {
                            var oldImg = Path.Combine(RootPath, productVM.Product.Image.TrimStart('\\'));
                          
                            if (System.IO.File.Exists(oldImg))
                            {
                                System.IO.File.Delete(oldImg);
                            }
                            
                        }

                        using (var FileStreamPlace = new FileStream(ImageFileFullPath, FileMode.Create))
                        {
                            Imgfile.CopyTo(FileStreamPlace);
                        }

                        productVM.Product.Image = @"Images\Products\" + uniqueFileName;
                    }
                    unitOfWork.Product.Update(productVM.Product);
                    unitOfWork.Complete();
                    TempData["Update"] = "Product has updated successfully";
                    return RedirectToAction("Index");
                }
            }
            return View(productVM.Product);
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Product productInDb = unitOfWork.Product.GetFirstOrDefault(c => c.Id == id); ;

            if (productInDb == null)
            {
                return Json(new { success = false, message="Error while Deleting..." });
            }
            unitOfWork.Product.Remove(productInDb);
            var oldImg = Path.Combine(webHostEnvironment.WebRootPath,productInDb.Image.TrimStart('\\'));

            if (System.IO.File.Exists(oldImg))
            {
                System.IO.File.Delete(oldImg);
            }
            unitOfWork.Complete();
            return Json(new { success = true, message = "File has been deleted" });
        }
    }
}

