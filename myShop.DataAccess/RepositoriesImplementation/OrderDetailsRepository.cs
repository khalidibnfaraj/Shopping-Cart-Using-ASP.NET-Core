using myShop.DataAccess.Data;
using myShop.DataAccess.RepositoriesImplementation;
using myShop.Entities.Models;
using myShop.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myShop.DataAccess.Implementation
{
    public class OrderDetailsRepository : GenericRepository<OrderDetails>, IOrderDetailsRepository
    {
        private readonly ApplicationDbContext _context;
        public OrderDetailsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(OrderDetails Orderdetail)
        {
            _context.OrderDetails.Update(Orderdetail);
        }
    }
}
