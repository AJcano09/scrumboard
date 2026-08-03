
using ScrumBoard.Application.Projects;
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

        // 1. NECESARIO PARA EF CORE: Constructor privado sin parámetros para la materialización desde la BD
        private Project()
        {
            
        }
      
        // Constructor de dominio para crear nuevos proyectos (con validaciones)
        public Project(Guid id, string name, string description, DateTime startDate, DateTime endDate, string statusName)
        {
            Id = id;
            SetValues(name, description, startDate, endDate, statusName);
        }

        public void Update(string name, string description, DateTime startDate, DateTime endDate, string statusName)
        {
            SetValues(name, description, startDate, endDate, statusName);
        }

        private void SetValues(string name, string description, DateTime startDate, DateTime endDate, string statusName)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ProjectValidationException("El nombre del proyecto es obligatorio.");

            if (string.IsNullOrWhiteSpace(description))
                throw new ProjectValidationException("La descripción del proyecto es obligatoria.");

            if (endDate < startDate)
                throw new ProjectValidationException("La fecha de fin no puede ser anterior a la fecha de inicio.");

            try
            {
                Status = ProjectStatus.FromName(statusName);
            }
            catch (ArgumentException)
            {
                throw new ProjectValidationException($"El estado '{statusName}' no es válido.");
            }

            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
        }

    }
}
