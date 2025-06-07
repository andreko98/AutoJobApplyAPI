using AutoJobApplyAPI.Services;
using AutoJobApplyDatabase.Context;
using AutoJobApplyDatabase.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoJobApplyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ScrapingService _scrapingService;

        public JobsController(AppDbContext context, ScrapingService scrapingService)
        {
            _context = context;
            _scrapingService = scrapingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobs()
        {
            return await _context.Jobs.OrderByDescending(j => j.DatePosted).ToListAsync();
        }

        [HttpPost("scrape")]
        public async Task<ActionResult> Scrape([FromQuery] string termo = "desenvolvedor")
        {
            var vagas = await _scrapingService.ScrapeInfoJobsAsync(termo);
            return Ok(new { total = vagas.Count });
        }
    }

}
