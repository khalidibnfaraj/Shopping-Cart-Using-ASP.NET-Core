using Microsoft.AspNetCore.Mvc;
using myShop.DataAccess.Data;
using myShop.Entities.Models;
using System.Runtime.ConstrainedExecution;

namespace myShop.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext _context)
        {
            this._context = _context;
        }
        public IActionResult Index()
        {
            List<Category> categories = _context.Categories.ToList();
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
                _context.Categories.Add(ctr);
                _context.SaveChanges();
                TempData["Create"] = "Category has added successfully";
                return RedirectToAction("Index");
            }
            return View("Create");
        }

        [HttpGet]
        public IActionResult Update(int? Id)
        {
            if(Id == null || Id == 0)
            {
                NotFound();
            }
            Category category = _context.Categories.Find(Id);
            return View(category);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Category ctr)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(ctr);
                _context.SaveChanges();
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

            Category category = _context.Categories.Find(id);
            return View(category);
        }
        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {
            Category category = _context.Categories.Find(id);

            if(category == null)
            {
                NotFound();
            }
            _context.Categories.Remove(category);
            _context.SaveChanges();
            TempData["Delete"] = "Category has deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
