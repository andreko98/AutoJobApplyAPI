using AutoJobApplyDatabase.Context;
using AutoJobApplyDatabase.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace AutoJobApplyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;
        private readonly OpenAiService _aiService;

        public ApplicationsController(AppDbContext context, EmailService emailService, OpenAiService aiService)
        {
            _context = context;
            _emailService = emailService;
            _aiService = aiService;
        }

        [HttpPost("apply")]
        public async Task<IActionResult> Apply(int userId, int jobId)
        {
            var user = await _context.Users.FindAsync(userId);
            var job = await _context.Jobs.FindAsync(jobId);

            if (user == null || job == null)
                return NotFound();

            var message = await _aiService.GenerateMessage(user, job);
            var result = await _emailService.SendEmail(user.Email, job.Company, message, user.CurriculoPath);

            var application = new Application
            {
                UserId = user.Id,
                JobId = job.Id,
                AppliedAt = DateTime.UtcNow,
                MessageSent = message,
                Status = result ? "Enviado" : "Erro"
            };

            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return Ok(application);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUser(int userId)
        {
            var applications = await _context.Applications
                .Include(a => a.Job)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();

            return Ok(applications);
        }

    }
}
