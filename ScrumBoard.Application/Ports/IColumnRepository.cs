using ScrumBoard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrumBoard.Application.Ports
{
    public interface IColumnRepository
    {
        Task<IEnumerable<Column>> GetProjectIdAsync(Guid projectId);
        Task<int> CountTasksByColumnIdAsync(Guid columnId);

    }
}
