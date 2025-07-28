using Microsoft.EntityFrameworkCore;
using Shared.Core.Entities;
using Shared.Core.Enums;

namespace TaskService.Infrastructure.Data
{
    public class SeedData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Email = "admin@adming.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = UserRole.Admin
            },
            new User
            {
                Id = 2,
                Email = "user@user.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
                Role = UserRole.Client
            }
            );

            modelBuilder.Entity<Shared.Core.Entities.Task>().HasData(
            new Shared.Core.Entities.Task
            {
                Id = 1,
                Title = "Complete project setup",
                Description = "Set up",
                DueDate = DateTime.UtcNow.AddDays(7),
                UserId = 2,
                IsCompleted = false
            }
        );
        }
    }
}
    
