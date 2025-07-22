using Shared.Core.Entities;

namespace TaskService.Infrastructure.Data
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
        System.Threading.Tasks.Task AddAsync(User user);
        System.Threading.Tasks.Task UpdateAsync(User user);
        System.Threading.Tasks.Task DeleteAsync(int id);
    }
}
