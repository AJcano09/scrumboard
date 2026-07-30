using Microsoft.EntityFrameworkCore;
using ScrumBoard.Domain.Entities;
using System;

namespace ScrumBoard.Infrastructure.Persistence
{
    public class ScrumDbContext : DbContext
    {
        public ScrumDbContext(DbContextOptions<ScrumDbContext> options) : base(options)
        {
            
        }

        public DbSet<User>  Users => Set<User>();

        public DbSet<Project> Projects => Set<Project>();
        public DbSet<Column> Columns => Set<Column>();

        public DbSet<Domain.Entities.Task> Tasks => Set<Domain.Entities.Task>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ScrumDbContext).Assembly); 
        }
    }
}
