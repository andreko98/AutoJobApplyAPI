using AutoJobApplyAPI.Models;
using AutoJobApplyDatabase.Context;
using AutoJobApplyDatabase.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AutoJobApplyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public UsersController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User updatedUser)
        {
            if (id != updatedUser.Id) return BadRequest();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Nome = updatedUser.Nome;
            user.Sobrenome = updatedUser.Sobrenome;
            user.Email = updatedUser.Email;
            user.DataNascimento = updatedUser.DataNascimento;
            user.Endereco = updatedUser.Endereco;
            user.Sobre = updatedUser.Sobre;

            if (!string.IsNullOrEmpty(updatedUser.CurriculoPath))
                user.CurriculoPath = updatedUser.CurriculoPath;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id}/upload")]
        public async Task<IActionResult> UploadCurriculo(int id, IFormFile file)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var uploadsFolder = Path.Combine("Uploads", "Curriculos");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            user.CurriculoPath = filePath;
            await _context.SaveChangesAsync();

            return Ok(new { path = filePath });
        }

    }
}
