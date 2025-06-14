using AutoJobApplyAPI.Services;
using AutoJobApplyDatabase.Entities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/Jobs")]
public class JobController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Job>>> GetJobs()
    {
        var jobs = await _jobService.GetJobsAsync();
        return Ok(jobs);
    }

    [HttpPost("Scrape/{search}")]
    public async Task<ActionResult> Scrape(string search)
    {
        var total = await _jobService.ScrapeJobsAsync(search);
        return Ok(new { total });
    }

    [HttpGet("Recent/{count}")]
    public async Task<ActionResult<IEnumerable<Job>>> GetRecentJobs(int count = 10)
    {
        var jobs = await _jobService.GetRecentJobsAsync(count);
        return Ok(jobs);
    }
}
