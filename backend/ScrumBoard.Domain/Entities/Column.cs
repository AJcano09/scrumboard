using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrumBoard.Domain.Entities
{
    public class Column
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Order { get; set; }

        public Guid ProjectId { get; set; }

        public Project Project { get; set; } = null!;

        public ICollection<Task> Tasks { get; set; } = [];
    }
}
