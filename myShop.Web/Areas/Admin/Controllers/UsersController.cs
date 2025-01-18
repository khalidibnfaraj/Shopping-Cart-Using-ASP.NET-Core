using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using myShop.DataAccess.Data;
using myShop.Entities.Models;
using myShop.Utilities;
using System.Security.Claims;


namespace myShop.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            //This code is for list all users but except the one who signed in.
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = claim.Value;
            var usersList = _context.ApplicationUsers.Where(x => x.Id != userId);
            return View(usersList);
        }

        public IActionResult LockUnLock(string? id)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(x => x.Id == id);
            if (user == null) { 
            return NotFound();
            }
            if (user.LockoutEnd == null || user.LockoutEnd < DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now.AddYears(1);
            }
            else
            {
                user.LockoutEnd = DateTime.Now;
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "Users", new { area = "Admin" });
        }

        [HttpGet]
        public IActionResult Delete(string? id)
        {
            ApplicationUser applicationUser = _context.ApplicationUsers.FirstOrDefault(u=>u.Id == id);
            return View(applicationUser);
        }
        [HttpPost]
        public IActionResult DeleteUser(string? Id)
        {
            ApplicationUser applicationUser = _context.ApplicationUsers.FirstOrDefault(u => u.Id == Id);
            _context.ApplicationUsers.Remove(applicationUser);
            _context.SaveChanges();
            return RedirectToAction("Index", "Users", new { area = "Admin" });
        }


    }
}
