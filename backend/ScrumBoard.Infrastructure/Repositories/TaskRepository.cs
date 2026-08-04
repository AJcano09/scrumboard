using Microsoft.EntityFrameworkCore;
using ScrumBoard.Application.Ports;
using ScrumBoard.Infrastructure.Persistence;

namespace ScrumBoard.Infrastructure.Repositories
{
    public class TaskRepository(ScrumBoardDbContext context) : Repository<Domain.Entities.Task>(context), ITaskRepository
    {

        public async Task<IEnumerable<Domain.Entities.Task>> GetByColumnIdAsync(Guid columnId) =>
            await _context.Tasks
                .Include(t=>t.Responsible)
            .Where(t => t.ColumnId == columnId)
            .OrderBy(t => t.Order)
            .ToListAsync();

      
    }
}
