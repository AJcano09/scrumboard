using Microsoft.EntityFrameworkCore;
using ScrumBoard.Application.Ports;
using ScrumBoard.Infrastructure.Persistence;

namespace ScrumBoard.Infrastructure.Repositories
{
    public class TaskRepository(ScrumBoardDbContext context) : Repository<Domain.Entities.Task>(context), ITaskRepository
    {
        private readonly ScrumBoardDbContext _context = context;

        public async Task<IEnumerable<Domain.Entities.Task>> GetByColumnIdASync(Guid columnId) =>
            await _context.Tasks
            .Where(t => t.ColumnId == columnId)
            .OrderBy(t => t.Order)
            .ToListAsync();
    }
}
