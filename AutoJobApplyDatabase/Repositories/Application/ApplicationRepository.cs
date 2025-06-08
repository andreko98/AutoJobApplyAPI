using AutoJobApplyDatabase.Context;
using AutoJobApplyDatabase.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoJobApplyDatabase.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly AppDbContext _context;

        public ApplicationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Application application)
        {
            await _context.Applications.AddAsync(application);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Application>> GetByUserIdAsync(int userId)
        {
            return await _context.Applications
                .Include(a => a.Job)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();
        }
    }

}
