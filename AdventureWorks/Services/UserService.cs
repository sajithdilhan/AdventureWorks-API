using AdventureWorks.Models.Identity;

namespace AdventureWorks.Services
{
    public class UserService : IUserService
    {
        private readonly List<AppUserDto> _users;

        public UserService()
        {
            _users = new List<AppUserDto>
        {
                new AppUserDto() { Id = Guid.NewGuid(), UserName = "user1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password1"), Email = "sajithdilhan@gmail.com" },
                new AppUserDto() { Id = Guid.NewGuid(), UserName = "user2", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password2"), Email = "xay@gmail.com", Role = "Admin" }
        };
        }

        public AppUserDto? GetUser(string? username)
        {
            ArgumentNullException.ThrowIfNull(username);

            return _users.SingleOrDefault(u => u.UserName == username);
        }

        public bool IsAuthenticated(string? password, string? passwordHash)
        {
            ArgumentNullException.ThrowIfNull(password);
            ArgumentNullException.ThrowIfNull(passwordHash);

            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
