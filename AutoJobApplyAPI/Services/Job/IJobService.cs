using AutoJobApplyDatabase.Entities;

namespace AutoJobApplyAPI.Services
{
    public interface IJobService
    {
        Task<List<Job>> GetJobsAsync();
        Task<List<Job>> ScrapeJobsAsync(string termo);
    }
}
