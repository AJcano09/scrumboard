

namespace ScrumBoard.Application.Ports
{
    public interface IProjectRepository : IRepository<Domain.Entities.Project>
    {
        Task<(IEnumerable<Domain.Entities.Project> items, int TotalCount)> GetPagedAsync(string searchTerm, int pageNumber, int pageSize);
    }
}
