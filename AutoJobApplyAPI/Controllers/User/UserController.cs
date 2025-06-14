using AutoJobApplyAPI.Models;
using AutoJobApplyAPI.Services;
using AutoJobApplyAPI.Services.Interface;
using AutoJobApplyDatabase.Context;
using AutoJobApplyDatabase.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AutoJobApplyAPI.Controllers
{
    [ApiController]
    [Route("api/Users")]
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
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null) return NotFound();
                return user;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult<User>> CreateUser(UserCreateRequest user)
        {
            try
            {
                var createdUser = await _userService.CreateUserAsync(user);
                return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserCreateRequest updatedUser)
        {
            try
            {
                if (id != updatedUser.Id) return BadRequest();

                var updated = await _userService.UpdateUserAsync(id, updatedUser);
                if (!updated) return NotFound();

                return NoContent();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost("{id}/UploadCV")]
        public async Task<IActionResult> UploadCV(int id, IFormFile file)
        {
            try
            {
                var path = await _userService.UploadCVAsync(id, file);
                if (path == null)
                    return BadRequest("Arquivo inválido ou usuário não encontrado.");

                return Ok(new { path });
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpGet("{id}/GetCVPath")]
        public async Task<IActionResult> GetCVPath(int id)
        {
            try
            {
                var path = await _userService.GetCVPathAsync(id);
                if (path.IsNullOrEmpty())
                    return BadRequest("Nenhum currículo encontrado.");

                return Ok(new { path });
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost("SaveEmailCredential")]
        public async Task<IActionResult> SaveEmailCredential([FromBody] EmailCredentialRequest request)
        {
            try
            {
                var user = await _userService.GetByIdAsync(request.UserId);
                if (user == null)
                    return NotFound("Usuário não encontrado.");

                var result = _emailService.SaveEmailCredential(request);

                if (result)
                    return Ok("Credencial cadastrada com sucesso.");
                else
                    return StatusCode(500, "Erro ao cadastrar credencial.");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
