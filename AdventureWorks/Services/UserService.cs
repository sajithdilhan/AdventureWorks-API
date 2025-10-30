using AdventureWorks.Models.Identity;
using AdventureWorks.Repositories;
using AdventureWorks.Utility;

namespace AdventureWorks.Services;

public class UserService(ILogger<UserService> logger, IUserRepository userRepository) : IUserService
{
    public bool IsAuthenticated(string? password, string? passwordHash)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(password);
            ArgumentNullException.ThrowIfNull(passwordHash);

            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to authenticate user. {}", ex.Message);
            throw;
        }       
    }

    public async Task<AppUserDto?> GetUserByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            return null;
        }

        var user = await userRepository.GetUserByIdAsync(id);
        if (user.Id == Guid.Empty)
        {
            return null;
        }
        return user.ToAppUserDto();
    }

    public async Task<AppUserDto?> GetUserByUsernameAsync(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return null;
        }

        var user = await userRepository.GetUserByUserNameAsync(username);
        if (user.Id == Guid.Empty)
        {
            return null;
        }
        return user.ToAppUserDto();
    }
}