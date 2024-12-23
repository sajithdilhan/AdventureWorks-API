using AdventureWorks.Models.Identity;

namespace AdventureWorks.Services
{
    public interface IUserService
    {
        Task<AppUserDto?> GetUserByUsernameAsync(string username);
        Task<AppUserDto?> GetUserByIdAsync(Guid id);
        bool IsAuthenticated(string? password, string? passwordHash);
    }
}
