using AutoJobApplyDatabase.Entities;

namespace AutoJobApplyDatabase.Repositories
{
    public interface IEmailRepository
    {
        Task AddAsync(EmailLog log);
        EmailCredential? GetEmailCredential(int userId);
        Task SaveEmailCredentialAsync(EmailCredential credential);
    }
}
