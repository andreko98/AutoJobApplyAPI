using AutoJobApplyAPI.Services;
using AutoJobApplyDatabase.Entities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ApplicationController : ControllerBase
{
    private readonly IApplicationService _applicationService;

    public ApplicationController(IApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [HttpPost("apply")]
    public async Task<IActionResult> Apply(int userId, int jobId)
    {
        try
        {
            var application = await _applicationService.ApplyAsync(userId, jobId);
            return Ok(application);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var applications = await _applicationService.GetApplicationsByUserAsync(userId);
        return Ok(applications);
    }

    [HttpGet("recent/{count}")]
    public async Task<ActionResult<IEnumerable<Application>>> GetRecentApplications(int count = 10)
    {
        var applications = await _applicationService.GetRecentApplicationsAsync(count);
        return Ok(applications);
    }
}