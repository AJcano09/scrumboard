using Microsoft.EntityFrameworkCore;
using ScrumBoard.Application.Ports;
using ScrumBoard.Domain.Entities;
using ScrumBoard.Infrastructure.Persistence;

namespace ScrumBoard.Infrastructure.Repositories
{
    public class ProjectRepository(ScrumBoardDbContext context) : Repository<Project>(context), IProjectRepository
    {
        public async Task<(IEnumerable<Project> items, int TotalCount)> GetPagedAsync(string searchTerm, int pageNumber, int pageSize)
        {
            var query = _context.Projects
                .Include(p=>p.Columns)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>  EF.Functions.ILike(p.Name,$"%{searchTerm}%"));
            }
            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(p=>p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);

        }
    }
}
