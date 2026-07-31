
using ScrumBoard.Infrastructure.Persistence;

namespace ScrumBoard.Infrastructure.Repositories
{
    public class Repository<T> where T : class
    {
        protected readonly ScrumDbContext _context;

        protected Repository(ScrumDbContext context)
        {
            _context = context;
        }

        public virtual async Task<T?> GetByIdAsync(Guid id) => await _context.Set<T>().FindAsync(id);

        public virtual async Task AddAsync(T entity)=> await _context.Set<T>().AddAsync(entity);

        public virtual Task UpdateASync(T entity)
        {
            _context.Set<T>().Update(entity);
            return Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(Guid id )
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
            }
        }
    }
}
