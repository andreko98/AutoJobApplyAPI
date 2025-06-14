using AutoJobApplyAPI.Models;

namespace AutoJobApplyAPI.Services.Interface
{
    public interface IEmailService
    {
        Task<bool> SendEmail(int userId, string toCompany, string jobTitle, string message, string attachmentPath);
        bool SaveEmailCredential(EmailCredentialRequest request);
    }
}
