

using Microsoft.EntityFrameworkCore;

namespace TaskService.Infrastructure.Data
{
    public class TaskRepository : ITaskRepository
    {

        private readonly TaskDbContext _context;

        public TaskRepository(TaskDbContext context) => _context = context;

        public async System.Threading.Tasks.Task AddAsync(Shared.Core.Entities.Task task)
        {
            await _context.Tasks.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task DeleteAsync(int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
            {
                throw new KeyNotFoundException($"Task with ID {taskId} not found.");
            }
            else {                 
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Shared.Core.Entities.Task>> GetAllAsync() => await _context.Tasks.ToListAsync()
            ?? throw new KeyNotFoundException("No tasks found.");


        public async Task<Shared.Core.Entities.Task> GetByIdAsync(int taskId) => await _context.Tasks.FindAsync(taskId) 
            ?? throw new KeyNotFoundException($"Task with ID {taskId} not found.");

        public async Task<IEnumerable<Shared.Core.Entities.Task>> GetByUserIdAsync(int userId) => await _context.Tasks
            .Where(t => t.UserId == userId)
            .ToListAsync() 
            ?? throw new KeyNotFoundException($"No tasks found for User ID {userId}.");

        public async System.Threading.Tasks.Task UpdateAsync(Shared.Core.Entities.Task task)
        {
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
        }
    }
}
