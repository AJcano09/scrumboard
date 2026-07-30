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
        Task<Project?> GetByIdAsync(Guid id);
        Task<(IEnumerable<Project> items, int TotalCount)> GetPagedASync(string searchTerm, int pageNumber, int pageSize);
        System.Threading.Tasks.Task AddASync(Project project);
        System.Threading.Tasks.Task UpdateAsync(Project project);
        System.Threading.Tasks.Task DeleteAsync(Guid id);
        
    }
}
