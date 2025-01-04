﻿using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Create(IEnumerable ctr)
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
            IEnumerable category = unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
            return View(category);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(IEnumerable ctr)
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

            IEnumerable category = unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
            return View(category);
        }
        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {
            IEnumerable category = unitOfWork.Category.GetFirstOrDefault(c => c.Id == id); ;

            if (category == null)
            {
                NotFound();
            }
            unitOfWork.Category.Remove(category);
            unitOfWork.Complete();
            TempData["Delete"] = "Category has deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
