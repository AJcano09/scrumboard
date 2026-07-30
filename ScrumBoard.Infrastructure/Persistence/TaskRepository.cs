using Microsoft.EntityFrameworkCore;
using ScrumBoard.Application.Ports;

namespace ScrumBoard.Infrastructure.Persistence
{
    public class TaskRepository : Repository<Domain.Entities.Task>, ITaskRepository
    {
        public TaskRepository(ScrumDbContext context) : base(context)
        {
            
        }
        public async Task<IEnumerable<Domain.Entities.Task>> GetByColumnIdASync(Guid columnId) =>
            await _context.Tasks
            .Where(t => t.ColumnId == columnId)
            .OrderBy(t => t.Order)
            .ToListAsync();
    }
}
