using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrumBoard.Application.Ports
{
    public interface ITaskRepository
    {
        Task<Domain.Entities.Task?> GetByIdAsync(Guid id);
        Task<IEnumerable<Domain.Entities.Task>> GetByColumnIdASync(Guid columnId);
        Task AddASync(Domain.Entities.Task task);
        Task UpdateASync(Domain.Entities.Task task);
        Task DeleteASync(Guid id);
    }
}
