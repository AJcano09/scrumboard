using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ScrumBoard.Domain.Entities;
using ScrumBoard.Infrastructure.Persistence.Converters;

namespace ScrumBoard.Infrastructure.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(150);
            builder.Property(p => p.Description).HasMaxLength(500);
            builder.Property(p => p.Status)
                .HasConversion<ProjectStatusConverter>()
                .HasColumnName("Status")
                .HasMaxLength(50)
                .IsRequired();

            builder.HasMany(p => p.Columns)
                .WithOne(c => c.Project)
                .HasForeignKey(c => c.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
