using Microsoft.EntityFrameworkCore;
using myShop.DataAccess.Data;
using myShop.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myShop.DataAccess.RepositoriesImplementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T: class 
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> _dbSet;
        public GenericRepository(ApplicationDbContext _context)
        {
            this._context = _context;
            _dbSet = _context.Set<T>();
        }
        public void Add(T entity)
        {
           _dbSet.Add(entity);
        }
        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public IEnumerable<T> GetAll(System.Linq.Expressions.Expression<Func<T, bool>>? filter = null, string? includeValue = null)
        {
            IQueryable<T> query = _dbSet; //_dbSet => Context.Category
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeValue != null)
            {
                string[] includes = includeValue.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in includes )
                {
                    query = query.Include(item.Trim());
                }
            }
            return query.ToList();
        }

        public T GetFirstOrDefault(System.Linq.Expressions.Expression<Func<T, bool>>? filter = null, string? includeValue = null)
        {
            IQueryable<T> query = _dbSet; //_dbSet => Context.Category
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeValue != null)
            {
                string[] includes = includeValue.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in includes)
                {
                    query = query.Include(item.Trim());
                }
            }
            return query.SingleOrDefault();
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

	
	}
}
