using Microsoft.EntityFrameworkCore;
using ScrumBoard.Application.Ports;
using ScrumBoard.Domain.Entities;
using ScrumBoard.Infrastructure.Persistence;

namespace ScrumBoard.Infrastructure.Repositories
{
    public class ColumnRepository(ScrumBoardDbContext context) : Repository<Column>(context), IColumnRepository
    {
        private readonly ScrumBoardDbContext _context = context;

        public async Task<int> CountTasksByColumnIdAsync(Guid columnId)=>
            await _context.Tasks.CountAsync(t => t.ColumnId == columnId);
        

        public async Task<IEnumerable<Column>> GetProjectIdAsync(Guid projectId)=>
            await _context.Columns.Where(c => c.ProjectId == projectId)
            .OrderBy(c=>c.Order)
            .ToListAsync();
    }
}
