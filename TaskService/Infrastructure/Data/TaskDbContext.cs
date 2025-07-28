using Microsoft.EntityFrameworkCore;
using Shared.Core.Entities;
using System.Security.Cryptography.X509Certificates;

namespace TaskService.Infrastructure.Data
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
        {
       
        }

        public DbSet<Shared.Core.Entities.Task> Tasks { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            SeedData.Seed(modelBuilder);
            // Configure User entity
            modelBuilder.Entity<User>()
                 .HasIndex(u => u.Email)
                 .IsUnique();
            // Configure Task entity
            modelBuilder.Entity<Shared.Core.Entities.Task>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.UserId);
            modelBuilder.Entity<Shared.Core.Entities.Task>()
                .Property(t => t.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}
