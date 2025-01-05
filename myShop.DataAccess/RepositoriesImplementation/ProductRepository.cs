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
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        private DbSet<Product> _products;

        public ProductRepository(ApplicationDbContext _context) : base(_context)
        {
            this._context = _context;
            _products = _context.Set<Product>();
        }

        public void Update(Product product)
        {
            var ProductIdinDb = _products.FirstOrDefault(c => c.Id == product.Id);
            if (ProductIdinDb != null)
            {
                ProductIdinDb.Name = product.Name;
                ProductIdinDb.Description = product.Description;
                ProductIdinDb.Image = product.Image;
                ProductIdinDb.Price = product.Price;
                ProductIdinDb.CategoryId = product.CategoryId;  
            }
        }
    }
}
