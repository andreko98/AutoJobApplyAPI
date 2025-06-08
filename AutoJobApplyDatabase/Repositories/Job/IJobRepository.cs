using AutoJobApplyDatabase.Entities;

namespace AutoJobApplyDatabase.Repositories
{
    public interface IJobRepository
    {
        Task<List<Job>> GetAllOrderedByDateDescAsync();
        Task AddRangeAsync(List<Job> jobs);
        Task<Job?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(string title, string company, string location);
    }
}