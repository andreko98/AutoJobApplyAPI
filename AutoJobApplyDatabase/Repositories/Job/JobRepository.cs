using Microsoft.EntityFrameworkCore;
using AutoJobApplyDatabase.Entities;
using AutoJobApplyDatabase.Context;

namespace AutoJobApplyDatabase.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly AppDbContext _context;
        public JobRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Job>> GetAllOrderedByDateDescAsync()
        {
            return await _context.Jobs.OrderByDescending(j => j.DatePosted).ToListAsync();
        }

        public async Task AddRangeAsync(List<Job> jobs)
        {
            await _context.Jobs.AddRangeAsync(jobs);
            await _context.SaveChangesAsync();
        }

        public async Task<Job?> GetByIdAsync(int id)
        {
            return await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task<bool> ExistsAsync(string title, string company, string location)
        {
            return await _context.Jobs.AnyAsync(j =>
                j.Title == title &&
                j.Company == company &&
                j.Location == location);
        }

        public async Task<List<Job>> GetRecentJobsAsync(int count)
        {
            return await _context.Jobs
                .OrderByDescending(j => j.DatePosted)
                .Take(count)
                .ToListAsync();
        }
    }
}
