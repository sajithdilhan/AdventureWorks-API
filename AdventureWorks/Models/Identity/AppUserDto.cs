using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AdventureWorks.Models.Identity
{
    public class AppUserDto
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        [JsonIgnore]
        public string Role { get; set; } = "User";

        public string UserName { get; set; } = string.Empty;
        [JsonIgnore]
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}
