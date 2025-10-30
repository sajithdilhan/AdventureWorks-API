using System.Text.Json.Serialization;

namespace AdventureWorks.Models.Identity;

public class AppUserDto
{
    [JsonIgnore]
    public Guid Id { get; set; }
    [JsonIgnore]
    public string Role { get; set; } = "User";
    [JsonPropertyName("username")]
    public string UserName { get; set; } = string.Empty;
    [JsonIgnore]
    public string Email { get; set; } = string.Empty;
    [JsonPropertyName("password")]
    public string PasswordHash { get; set; } = string.Empty;
}