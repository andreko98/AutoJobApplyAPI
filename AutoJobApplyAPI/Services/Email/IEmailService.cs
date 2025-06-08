namespace AutoJobApplyAPI.Services.Interface
{
    public interface IEmailService
    {
        Task<bool> SendEmail(string fromEmail, string toCompany, string jobTitle, string message, string attachmentPath);
    }
}
