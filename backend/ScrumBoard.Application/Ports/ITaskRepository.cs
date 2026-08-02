
namespace ScrumBoard.Application.Ports
{
    public interface ITaskRepository :IRepository<Domain.Entities.Task>
    {
        Task<IEnumerable<Domain.Entities.Task>> GetByColumnIdAsync(Guid columnId);
      
    }
}
