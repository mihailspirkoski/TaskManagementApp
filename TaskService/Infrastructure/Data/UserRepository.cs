using Microsoft.EntityFrameworkCore;
using Shared.Core.Entities;

namespace TaskService.Infrastructure.Data
{
    public class UserRepository : IUserRepository
    {

        private readonly TaskDbContext _context;

        public UserRepository(TaskDbContext context) => _context = context;

        public async System.Threading.Tasks.Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync() => await _context.Users.ToListAsync();


        public async Task<User> GetByEmailAsync(string email) => await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);


        public async Task<User> GetByIdAsync(int id) => await _context.Users.FindAsync(id) 
            ?? throw new KeyNotFoundException($"User with ID {id} not found.");


        public async System.Threading.Tasks.Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
