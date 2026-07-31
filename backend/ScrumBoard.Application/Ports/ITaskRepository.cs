using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrumBoard.Application.Ports
{
    public interface ITaskRepository
    {
        Task<IEnumerable<Domain.Entities.Task>> GetByColumnIdASync(Guid columnId);
      
    }
}
