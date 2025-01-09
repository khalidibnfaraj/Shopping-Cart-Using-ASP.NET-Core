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
    public class ShoppingCartRepository : GenericRepository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _context;
        private DbSet<ShoppingCart> _shoppingCarts;

        public ShoppingCartRepository(ApplicationDbContext _context) : base(_context)
        {
            this._context = _context;
            _shoppingCarts= _context.Set<ShoppingCart>();
        }
    }
}
