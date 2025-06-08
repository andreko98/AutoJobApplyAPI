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

    [HttpPost("scrape")]
    public async Task<ActionResult> Scrape([FromQuery] string termo = "desenvolvedor")
    {
        var total = await _jobService.ScrapeJobsAsync(termo);
        return Ok(new { total });
    }
}
