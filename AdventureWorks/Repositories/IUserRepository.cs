using AdventureWorks.Models.Identity;

namespace AdventureWorks.Repositories
{
    public interface IUserRepository
    {
        public Task<User> GetUserByIdAsync(Guid id);
        public Task<User> GetUserByUserNameAsync(string userName);
        public Task<User> GetUserByEmailAsync(string email);
        public Task AddUserAsync(User user);
        public Task<User> UpdateUserAsync(User user);
        public Task<bool> DeleteUserAsync(Guid id);
    }
}
