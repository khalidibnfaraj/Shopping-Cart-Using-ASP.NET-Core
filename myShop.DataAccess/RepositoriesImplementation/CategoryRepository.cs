using Microsoft.EntityFrameworkCore;
using myShop.DataAccess.Data;
using myShop.Entities.Models;
using myShop.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myShop.DataAccess.RepositoriesImplementation
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        private DbSet<Category> _categories;

        public CategoryRepository(ApplicationDbContext _context) : base(_context)
        {
            this._context = _context;
            _categories= _context.Set<Category>();
        }

        public void Update(Category category)
        {
            var CategoryIdinDb = _categories.FirstOrDefault(c=>c.Id == category.Id);
            if (CategoryIdinDb != null)
            {
                CategoryIdinDb.Name = category.Name;
                CategoryIdinDb.Description = category.Description;
                CategoryIdinDb.CreatedTime = DateTime.Now;
            }
        }
    }
}
