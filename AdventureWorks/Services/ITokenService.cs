using AdventureWorks.Models.Identity;

namespace AdventureWorks.Services
{
    public interface ITokenService
    {
       public string GenerateToken(AppUserDto? user);
    }
}
