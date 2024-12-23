using AdventureWorks.Models.Identity;
using AdventureWorks.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdventureWorks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ITokenService tokenService, ILogger<UserController> logger)
        {
            _tokenService = tokenService;
            _userService = userService;
            _logger = logger;
        }

        [HttpPost]
        [Route("token")]
        [AllowAnonymous]
        public IActionResult Token([FromBody] AppUserDto user)
        {
            if (user == null || user.UserName == null || user.PasswordHash == null)
            {
                return BadRequest();
            }

            try
            {
                var storedUser = _userService.GetUser(user?.UserName);
                if (!_userService.IsAuthenticated(user?.PasswordHash, storedUser?.PasswordHash))
                {
                    return Unauthorized();
                }

                var tokenString = _tokenService.GenerateToken(storedUser);
                return Ok(new { token = tokenString });
            }
            catch (Exception)
            {
                _logger.LogError("");
                return Problem();
            }

        }
    }
}
