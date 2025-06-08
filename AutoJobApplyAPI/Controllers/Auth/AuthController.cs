using AutoJobApplyAPI.Services.Interface;
using AutoJobApplyDatabase.Context;
using Microsoft.AspNetCore.Mvc;

namespace AutoJobApplyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(AppDbContext context, IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
                return NotFound(new { message = "Usuário não encontrado." });

            return Ok(user);
        }

    }
}
