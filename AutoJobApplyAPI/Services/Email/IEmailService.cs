namespace AutoJobApplyAPI.Services.Interface
{
    public interface IEmailService
    {
        Task<bool> SendEmail(int userId, string toCompany, string jobTitle, string message, string attachmentPath);
        Task<bool> SaveEmailCredential(int userId, string email, string password);
    }
}
