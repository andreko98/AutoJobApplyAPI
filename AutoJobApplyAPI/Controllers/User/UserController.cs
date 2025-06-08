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
        private readonly IEmailService _emailService;

        public UsersController(AppDbContext context, IWebHostEnvironment env, IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
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

        [HttpPost("{id}/uploadCV")]
        public async Task<IActionResult> UploadCV(int id, IFormFile file)
        {
            var path = await _userService.UploadCVAsync(id, file);
            if (path == null)
                return BadRequest("Arquivo inválido ou usuário não encontrado.");

            return Ok(new { path });
        }

        [HttpPost("SaveEmailCredential")]
        public async Task<IActionResult> SaveEmailCredential(int id, [FromBody] EmailCredentialRequest request)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound("Usuário não encontrado.");

            var result = await _emailService.SaveEmailCredential(id, request.Email, request.Password);

            if (result)
                return Ok("Credencial cadastrada com sucesso.");
            else
                return StatusCode(500, "Erro ao cadastrar credencial.");
        }
    }
}
