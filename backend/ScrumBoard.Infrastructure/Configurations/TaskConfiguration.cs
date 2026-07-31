using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ScrumBoard.Infrastructure.Configurations
{
    public class TaskConfiguration : IEntityTypeConfiguration<Domain.Entities.Task>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Task> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
            builder.Property(t => t.Description).HasMaxLength(1000);
            builder.Property(t => t.Priority).IsRequired().HasMaxLength(50);
            builder.Property(t => t.Order).IsRequired();
            builder.Property(t => t.CreatedAt).IsRequired();

            builder.HasOne(t => t.Responsible)
                .WithMany()
                .HasForeignKey(t => t.ResponsibleId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
