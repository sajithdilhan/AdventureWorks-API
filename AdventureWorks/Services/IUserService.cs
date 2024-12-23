using AdventureWorks.Models.Identity;

namespace AdventureWorks.Services
{
    public interface IUserService
    {
        AppUserDto? GetUser(string? username);
        bool IsAuthenticated(string? password, string? passwordHash);
    }
}
