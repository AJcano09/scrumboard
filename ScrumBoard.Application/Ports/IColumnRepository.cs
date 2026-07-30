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
        Task<Column?> GetByIdAsync(Guid id);
        Task<IEnumerable<Column>> GetProjectIdASync(Guid projectId);

        System.Threading.Tasks.Task AddASync(Column column);
        System.Threading.Tasks.Task UpdateASync(Column column);

        System.Threading.Tasks.Task DeleteASync(Guid id);
        Task<int> CountTasksByColunIdASync(Guid ColumnId);

    }
}
