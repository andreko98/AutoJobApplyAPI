using AutoJobApplyAPI.Models;
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

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.GetByEmailAsync(request.Email);

            const string genericError = "E-mail ou senha incorretos";

            if (user == null)
                return Unauthorized(new { message = genericError });

            var isValid = await _userService.ValidatePasswordAsync(user.Id, request.Password);
            if (!isValid)
                return Unauthorized(new { message = genericError });

            return Ok(user);
        }
    }
}
