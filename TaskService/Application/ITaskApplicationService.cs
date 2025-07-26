using Shared.Core.Entities;
using TaskService.Application.DTOs;

namespace TaskService.Application
{
    public interface ITaskApplicationService
    {
        Task<Shared.Core.Entities.Task> GetByIdAsync(int id);
        Task<IEnumerable<Shared.Core.Entities.Task>> GetAllAsync();
        Task <IEnumerable<Shared.Core.Entities.Task>> GetByUserIdAsync(int userId);
        System.Threading.Tasks.Task<Shared.Core.Entities.Task> AddAsync(CreateTaskDto dto, int userId);
        System.Threading.Tasks.Task UpdateAsync(UpdateTaskDto dto, int userId);
        System.Threading.Tasks.Task DeleteAsync(int id, int userId);
    }
}
