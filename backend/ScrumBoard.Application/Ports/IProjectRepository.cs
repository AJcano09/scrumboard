using ScrumBoard.Domain.Entities;

namespace ScrumBoard.Application.Ports
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<(IEnumerable<Project> items, int TotalCount)> GetPagedAsync(string searchTerm, int pageNumber, int pageSize);
    }
}
