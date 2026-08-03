using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScrumBoard.Domain.Enums;

namespace ScrumBoard.Domain.Entities
{
    public class Project
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public ProjectStatus Status { get; set; } = ProjectStatus.Pending;

        public ICollection<Column> Columns { get; set; } = [];

    }
}
