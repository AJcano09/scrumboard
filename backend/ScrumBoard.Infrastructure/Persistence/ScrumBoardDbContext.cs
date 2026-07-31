using Microsoft.EntityFrameworkCore;
using ScrumBoard.Domain.Entities;
using System;

namespace ScrumBoard.Infrastructure.Persistence
{
    public class ScrumBoardDbContext(DbContextOptions<ScrumBoardDbContext> options) : DbContext(options)
    {
        public DbSet<User>  Users => Set<User>();

        public DbSet<Project> Projects => Set<Project>();
        public DbSet<Column> Columns => Set<Column>();

        public DbSet<Domain.Entities.Task> Tasks => Set<Domain.Entities.Task>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ScrumBoardDbContext).Assembly); 
        }
    }
}
