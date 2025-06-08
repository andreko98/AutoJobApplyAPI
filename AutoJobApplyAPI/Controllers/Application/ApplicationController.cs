using AutoJobApplyAPI.Services;
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
}