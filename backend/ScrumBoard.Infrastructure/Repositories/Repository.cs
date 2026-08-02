
using Microsoft.EntityFrameworkCore;
using ScrumBoard.Application.Ports;
using ScrumBoard.Infrastructure.Persistence;

namespace ScrumBoard.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ScrumBoardDbContext _context;

        protected Repository(ScrumBoardDbContext context)
        {
            _context = context;
        }


        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
          return await _context.Set<T>().FindAsync(id);
        }

        public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetAllPagedAsync(int pageNumber, int pageSize)
        {
            var query = _context.Set<T>().AsNoTracking();
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }


        public virtual async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

    }
}
