using Microsoft.EntityFrameworkCore;
using ScrumBoard.Application.Ports;
using ScrumBoard.Domain.Entities;


namespace ScrumBoard.Infrastructure.Persistence
{
    internal class ColumnRepository : Repository<Column>, IColumnRepository
    {
        public ColumnRepository(ScrumDbContext context) : base(context)
        {
            
        }

        public async Task<int> CountTasksByColumnIdAsync(Guid columnId)=>
            await _context.Tasks.CountAsync(t => t.ColumnId == columnId);
        

        public async Task<IEnumerable<Column>> GetProjectIdAsync(Guid projectId)=>
            await _context.Columns.Where(c => c.ProjectId == projectId)
            .OrderBy(c=>c.Order)
            .ToListAsync();
    }
}
