using System.Net;
using System.Net.Mail;

namespace AutoJobApplyAPI.Services
{
    public class EmailService
    {
        public async Task<bool> SendEmail(string fromEmail, string toCompany, string message, string attachmentPath)
        {
            try
            {
                var mail = new MailMessage();
                mail.From = new MailAddress(fromEmail);
                mail.To.Add("recrutamento@" + toCompany.ToLower() + ".com");
                mail.Subject = "Candidatura para vaga";
                mail.Body = message;

                if (!string.IsNullOrEmpty(attachmentPath))
                    mail.Attachments.Add(new Attachment(attachmentPath));

                using var smtp = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromEmail, "SENHA_DO_USUARIO"),
                    EnableSsl = true
                };

                await smtp.SendMailAsync(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
