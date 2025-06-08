using AutoJobApplyDatabase.Context;
using AutoJobApplyDatabase.Entities;

namespace AutoJobApplyDatabase.Repositories
{
    public class EmailRepository : IEmailRepository
    {
        private readonly AppDbContext _context;

        public EmailRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(EmailLog log)
        {
            _context.EmailLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }

}
