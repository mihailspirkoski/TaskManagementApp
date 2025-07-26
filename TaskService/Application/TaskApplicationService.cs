using Shared.Core.Entities;
using TaskService.Application.DTOs;
using TaskService.Infrastructure.Data;

namespace TaskService.Application
{
    public class TaskApplicationService : ITaskApplicationService
    {

        private readonly ITaskRepository _taskRepository;

        public TaskApplicationService(ITaskRepository taskRepository) => _taskRepository = taskRepository;

        public async System.Threading.Tasks.Task<Shared.Core.Entities.Task> AddAsync(CreateTaskDto dto, int userId)
        {
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Description))
                throw new ArgumentException("Title and description are required");

            var task = new Shared.Core.Entities.Task
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                UserId = userId
            };
            await _taskRepository.AddAsync(task);
            return task;
        }

        public async System.Threading.Tasks.Task DeleteAsync(int id, int userId)
        {
            var task = await _taskRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Task not found");
            if (task.UserId != userId)
                throw new UnauthorizedAccessException("You do not have permission to delete this task");
            await _taskRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Shared.Core.Entities.Task>> GetAllAsync() => await _taskRepository.GetAllAsync();

        public async Task<Shared.Core.Entities.Task> GetByIdAsync(int id)
            => await _taskRepository.GetByIdAsync(id) ?? throw new KeyNotFoundException("Task not found");

        public async Task<IEnumerable<Shared.Core.Entities.Task>> GetByUserIdAsync(int userId)
            => await _taskRepository.GetByUserIdAsync(userId);

        public async System.Threading.Tasks.Task UpdateAsync(UpdateTaskDto dto, int userId)
        {
            if (string.IsNullOrWhiteSpace(dto.Title) || string.IsNullOrWhiteSpace(dto.Description))
                throw new ArgumentException("Title and description are required");

            var task = await _taskRepository.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException("Task not found");
            if(task.UserId != userId) throw new UnauthorizedAccessException("You do not have permission to update this task");

            task.Title = dto.Title;
            task.Description = dto.Description;
            task.DueDate = dto.DueDate;
            task.IsCompleted = dto.IsCompleted;
            task.CompletedAt = dto.IsCompleted ? DateTime.UtcNow : null;
            await _taskRepository.UpdateAsync(task);
        }
    }
}
