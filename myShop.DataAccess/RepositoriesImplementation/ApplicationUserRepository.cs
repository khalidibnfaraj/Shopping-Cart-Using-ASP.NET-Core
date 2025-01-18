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
	public class ApplicationUserRepository : GenericRepository<ApplicationUser>, IApplicationUserRepository
	{
		private readonly ApplicationDbContext _context;
		private DbSet<ApplicationUser> _applicationUsers;

		public ApplicationUserRepository(ApplicationDbContext _context) : base(_context)
		{
			this._context = _context;
			_applicationUsers = _context.Set<ApplicationUser>();
		}

	}
}
    
