using ScrumBoard.Domain.Entities;

namespace ScrumBoard.Application.Ports
{
    public interface IColumnRepository : IRepository<Column>
    {
        Task<IEnumerable<Column>> GetProjectIdAsync(Guid projectId);
        Task<int> CountTasksByColumnIdAsync(Guid columnId);

    }
}
