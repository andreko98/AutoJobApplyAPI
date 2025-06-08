using AutoJobApplyAPI.Services;
using AutoJobApplyDatabase.Entities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
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

    [HttpPost("scrape/{search}")]
    public async Task<ActionResult> Scrape(string search)
    {
        var total = await _jobService.ScrapeJobsAsync(search);
        return Ok(new { total });
    }

    [HttpGet("recent/{count}")]
    public async Task<ActionResult<IEnumerable<Job>>> GetRecentJobs(int count = 10)
    {
        var jobs = await _jobService.GetRecentJobsAsync(count);
        return Ok(jobs);
    }
}
