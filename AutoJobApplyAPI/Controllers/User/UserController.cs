using AutoJobApplyAPI.Models;
using AutoJobApplyAPI.Services;
using AutoJobApplyAPI.Services.Interface;
using AutoJobApplyDatabase.Context;
using AutoJobApplyDatabase.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AutoJobApplyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(AppDbContext context, IWebHostEnvironment env, IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User updatedUser)
        {
            if (id != updatedUser.Id) return BadRequest();

            var updated = await _userService.UpdateUserAsync(id, updatedUser);
            if (!updated) return NotFound();

            return NoContent();
        }

        [HttpPost("{id}/upload")]
        public async Task<IActionResult> UploadCurriculo(int id, IFormFile file)
        {
            var path = await _userService.UploadCurriculoAsync(id, file);
            if (path == null)
                return BadRequest("Arquivo inválido ou usuário não encontrado.");

            return Ok(new { path });
        }
    }
}
