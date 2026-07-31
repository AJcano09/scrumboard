using ScrumBoard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrumBoard.Application.Ports
{
    public interface IProjectRepository
    {
        Task<(IEnumerable<Project> items, int TotalCount)> GetPagedASync(string searchTerm, int pageNumber, int pageSize);
    }
}
