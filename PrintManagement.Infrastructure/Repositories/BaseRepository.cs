using Microsoft.EntityFrameworkCore;
using PrintManagement.Domain.IRepositories;
using PrintManagement.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        public BaseRepository(ApplicationDbContext context) 
        {
            _context = context;
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> expression = null)
        {
            return await _context.SetEntity<T>().CountAsync(expression);
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            _context.SetEntity<T>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<string> DeleteAsync(int Id)
        {
            var entity = await _context.SetEntity<T>().FindAsync(Id);
            if (entity == null) 
            {
                return "Not Found";
            }
            _context.SetEntity<T>().Remove(entity);
            await _context.SaveChangesAsync();
            return "Delete Success";
        }

        public async Task<IQueryable<T>> GetAllAsync(Expression<Func<T, bool>> expression = null)
        {
            IQueryable<T> query = expression != null 
                ? _context.SetEntity<T>().Where(expression) : _context.SetEntity<T>().AsQueryable();
            return query;
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> expression)
        {
            return await _context.SetEntity<T>().FirstOrDefaultAsync(expression);
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            _context.SetEntity<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
