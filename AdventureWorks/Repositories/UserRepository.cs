using AdventureWorks.Database;
using AdventureWorks.Models.Identity;
using Microsoft.EntityFrameworkCore;

namespace AdventureWorks.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ApplicationDbContext dbContext, ILogger<UserRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task AddUserAsync(User user)
        {
           await  _dbContext.AppUsers.AddAsync(user);
           await _dbContext.SaveChangesAsync();
           _logger.LogInformation("Saved user : {}", user.UserName);
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await GetUserByIdAsync(id);
            if (user.Id == Guid.Empty)
            {
                return false;
            }
            _dbContext.AppUsers.Remove(user);
            int result = await _dbContext.SaveChangesAsync();
            if (result > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _dbContext.AppUsers.FirstOrDefaultAsync(u => u.Email == email);
                if (user != null) return user;
                _logger.LogWarning("User with Email {} not found in database", email);
                return new User();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get user from database with Email {} {}", email, ex.Message);
                throw;
            }
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            try
            {
                var user = await _dbContext.AppUsers.FindAsync(id);
                if (user != null) return user;
                _logger.LogWarning("User with ID {} not found in database", id);
                return new User();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get user from database with ID {} {}", id, ex.Message);
                throw;
            }
        }

        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            try
            {
                var user =  await _dbContext.AppUsers.FirstOrDefaultAsync(u => u.UserName == userName);
                if (user != null) return user;
                _logger.LogWarning("User with Username {} not found in database", userName);
                return new User();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get user from database with Username {} {}", userName, ex.Message);
                throw;
            }
        }

        public Task<User> UpdateUserAsync(User user)
        {
            _dbContext.AppUsers.Update(user);
            _dbContext.SaveChanges();
            return Task.FromResult(user);
        }
    }
}
