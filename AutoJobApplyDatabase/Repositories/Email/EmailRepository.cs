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

        public EmailCredential? GetEmailCredential(int userId)
        {
            return _context.EmailCredentials
                .FirstOrDefault(e => e.UserId == userId);
        }

        public async Task SaveEmailCredentialAsync(EmailCredential credential)
        {
            var existing = _context.EmailCredentials.FirstOrDefault(e => e.UserId == credential.UserId);

            if (existing == null)
            {
                _context.EmailCredentials.Add(credential);
            }
            else
            {
                existing.Email = credential.Email;
                existing.EncryptedPassword = credential.EncryptedPassword;
                _context.EmailCredentials.Update(existing);
            }

            await _context.SaveChangesAsync();
        }
    }
}
