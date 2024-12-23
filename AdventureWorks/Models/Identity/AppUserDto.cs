using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AdventureWorks.Models.Identity
{
    public class AppUserDto
    {
        public Guid Id { get; set; }
        public string? Role { get; set; } = "User";

        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}
