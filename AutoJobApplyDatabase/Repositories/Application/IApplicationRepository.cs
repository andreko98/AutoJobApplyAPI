using AutoJobApplyDatabase.Entities;

namespace AutoJobApplyDatabase.Repositories
{
    public interface IApplicationRepository
    {
        Task AddAsync(Application application);
        Task<List<Application>> GetByUserIdAsync(int userId);
    }
}
