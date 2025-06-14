using AutoJobApplyDatabase.Entities;

namespace AutoJobApplyDatabase.Repositories
{
    public interface IEmailRepository
    {
        Task AddAsync(EmailLog log);
        EmailCredential? GetEmailCredential(int userId);
        bool SaveEmailCredential(EmailCredential credential);
    }
}
