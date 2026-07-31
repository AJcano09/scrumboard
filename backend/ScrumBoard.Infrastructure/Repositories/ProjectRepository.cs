using Microsoft.EntityFrameworkCore;
using ScrumBoard.Application.Ports;
using ScrumBoard.Domain.Entities;
using ScrumBoard.Infrastructure.Persistence;

namespace ScrumBoard.Infrastructure.Repositories
{
    public class ProjectRepository(ScrumBoardDbContext context) : Repository<Project>(context), IProjectRepository
    {
        private readonly  ScrumBoardDbContext _context = context;

        public async Task<(IEnumerable<Project> items, int TotalCount)> GetPagedASync(string searchTerm, int pageNumber, int pageSize)
        {
            var query = _context.Projects.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p => p.Name.ToLower().Contains(searchTerm));
            }
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);

        }
    }
}
