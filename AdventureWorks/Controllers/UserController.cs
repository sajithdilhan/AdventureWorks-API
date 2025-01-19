using AdventureWorks.Models.Identity;
using AdventureWorks.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdventureWorks.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(IUserService userService, ITokenService tokenService, ILogger<UserController> logger) : ControllerBase
{
    [HttpPost]
    [Route("token")]
    [AllowAnonymous]
    public async Task<IActionResult> Token([FromBody] AppUserDto user)
    {
        if (user == null || user.UserName == null || user.PasswordHash == null)
        {
            return BadRequest();
        }

        try
        {
            var storedUser = await userService.GetUserByUsernameAsync(user.UserName);
            if (storedUser is null || !userService.IsAuthenticated(user?.PasswordHash, storedUser.PasswordHash))
            {
                return Unauthorized();
            }

            var tokenString = tokenService.GenerateToken(storedUser);
            return Ok(new { token = tokenString });
        }
        catch (Exception)
        {
            logger.LogError("Failed to authenticate user. {}", user.UserName);
            return Unauthorized();
        }

    }
}