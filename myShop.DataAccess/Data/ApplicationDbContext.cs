using Microsoft.EntityFrameworkCore;
using myShop.Entities.Models;

namespace myShop.DataAccess.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {

        }

        public DbSet<IEnumerable> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
