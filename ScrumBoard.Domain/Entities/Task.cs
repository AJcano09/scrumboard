using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrumBoard.Domain.Entities
{
    public class Task
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Priority { get; set; } = string.Empty;

        public Guid ResponsibleId { get; set; }

        public Guid ColumnId { get; set; }

        public int Order { get; set; }

        public DateTime CreateAt { get; set; }

        public User Responsible { get; set; } = null!;

        public Column Column { get; set; } = null!;

    }
}
