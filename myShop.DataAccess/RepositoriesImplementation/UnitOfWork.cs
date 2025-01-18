using Microsoft.EntityFrameworkCore;
using myShop.DataAccess.Data;
using myShop.DataAccess.Implementation;
using myShop.Entities.Models;
using myShop.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myShop.DataAccess.RepositoriesImplementation
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly ApplicationDbContext _context;
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }

		public IOrderHeaderRepository OrderHeader { get; private set; }

		public IOrderDetailsRepository OrderDetails { get; private set; }

        public IApplicationUserRepository ApplicationUser { get; private set; }

		public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Category = new CategoryRepository(context);
            Product = new ProductRepository(context);
            ShoppingCart = new ShoppingCartRepository(context);
            OrderHeader = new OrderHeaderRepository(context);
            OrderDetails = new OrderDetailsRepository(context);
            ApplicationUser = new ApplicationUserRepository(context);
        }

        public int Complete()
        {
           return _context.SaveChanges(); 
        }
        

        public void Dispose()
        {
            _context.Dispose();
        }

		
	}
}
