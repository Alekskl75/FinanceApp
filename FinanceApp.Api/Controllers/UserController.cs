using FinanceApp.Application.Interfacies;
using FinanceApp.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceApp.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController(IUserService userService, IConfiguration config) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            try
            {
                var userId = await userService.RegisterAsync(command);
                return CreatedAtAction(nameof(GetUser), new { id = userId }, userId);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserQuery query)
        {
            try
            {
                var token = await userService.LoginAsync(query);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            await userService.LogoutAsync(userId);
            return NoContent();
        }

        [HttpPut("favorites")]
        [Authorize]
        public async Task<IActionResult> UpdateFavorites([FromBody] List<string> favorites)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var command = new UpdateFavoritesCommand(userId, favorites);

            try
            {
                await userService.UpdateFavoritesAsync(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetUser(Guid id)
        {
            // Здесь можно добавить логику получения данных пользователя
            return Ok(new { id });
        }
    }

}
