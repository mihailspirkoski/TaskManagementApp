using Shared.Core.Entities;

namespace TaskService.Infrastructure.Data
{
    public interface ITaskRepository
    {
        Task<Shared.Core.Entities.Task> GetByIdAsync(int taskId);
        Task<IEnumerable<Shared.Core.Entities.Task>> GetAllAsync();
        Task<IEnumerable<Shared.Core.Entities.Task>> GetByUserIdAsync(int userId);
        System.Threading.Tasks.Task AddAsync(Shared.Core.Entities.Task task);
        System.Threading.Tasks.Task UpdateAsync(Shared.Core.Entities.Task task);
        System.Threading.Tasks.Task DeleteAsync(int taskId);
    }
}
