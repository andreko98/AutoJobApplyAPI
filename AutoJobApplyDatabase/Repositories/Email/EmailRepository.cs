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

        public bool SaveEmailCredential(EmailCredential credential)
        {
            var saveCredential = _context.EmailCredentials.FirstOrDefault(e => e.UserId == credential.UserId);

            if (saveCredential == null)
            {
                _context.EmailCredentials.Add(credential);
                _context.SaveChanges();

                saveCredential = _context.EmailCredentials.FirstOrDefault(e => e.UserId == credential.UserId);

                var user = _context.Users.FirstOrDefault(u => u.Id == credential.UserId);

                if (user != null)
                {
                    user.EmailCredentialId = saveCredential!.Id;
                }
            }
            else
            {
                saveCredential.Email = credential.Email;
                saveCredential.EncryptedPassword = credential.EncryptedPassword;
                _context.EmailCredentials.Update(saveCredential);
            }

            _context.SaveChanges();

            return true;
        }
    }
}
