using Microsoft.AspNetCore.Mvc;
using myShop.DataAccess.Data;
using myShop.DataAccess.RepositoriesImplementation;
using myShop.Entities.Models;
using myShop.Entities.Repositories;
using System.Runtime.ConstrainedExecution;

namespace myShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private IUnitOfWork unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var categories = unitOfWork.Category.GetAll();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category ctr)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Category.Add(ctr);
                unitOfWork.Complete();
                TempData["Create"] = "Category has added successfully";
                return RedirectToAction("Index");
            }
            return View("Create");
        }

        [HttpGet]
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
            {
                NotFound();
            }
            Category category = unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
            return View(category);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Category ctr)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Category.Update(ctr);
                unitOfWork.Complete();
                TempData["Update"] = "Category has updated successfully";
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

            Category category = unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
            return View(category);
        }
        [HttpPost]
        public IActionResult DeleteCategory(int? Id)
        {
            var categoryIndb = unitOfWork.Category.GetFirstOrDefault(x => x.Id ==Id);
            if (categoryIndb == null)
            {
                NotFound();
            }
            unitOfWork.Category.Remove(categoryIndb);
            //_context.Categories.Remove(categoryIndb);
            //_context.SaveChanges();
            unitOfWork.Complete();
            TempData["Delete"] = "Item has Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
