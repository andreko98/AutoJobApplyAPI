using AutoJobApplyDatabase.Entities;

namespace AutoJobApplyAPI.Services
{
    public interface IApplicationService
    {
        Task<Application> ApplyAsync(int userId, int jobId);
        Task<List<Application>> GetApplicationsByUserAsync(int userId);
    }
}
