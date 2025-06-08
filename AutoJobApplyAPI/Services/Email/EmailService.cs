using AutoJobApplyAPI.Models;
using AutoJobApplyAPI.Services.Interface;
using AutoJobApplyDatabase.Entities;
using AutoJobApplyDatabase.Repositories;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace AutoJobApplyAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly IEmailRepository _emailRepository;

        public EmailService(IOptions<EmailSettings> options, IEmailRepository emailRepository)
        {
            _settings = options.Value;
            _emailRepository = emailRepository;
        }

        public async Task<bool> SendEmail(string fromEmail, string toCompany, string jobTitle, string message, string attachmentPath)
        {
            foreach (var pattern in _settings.EmailPatterns)
            {
                var toEmail = pattern.Replace("{company}", toCompany.ToLower());

                var emailLog = new EmailLog
                {
                    FromEmail = fromEmail,
                    ToEmail = toEmail,
                    Subject = $"Candidatura para vaga: {jobTitle}",
                    Body = message,
                    AttachmentPath = attachmentPath,
                    SentAt = DateTime.UtcNow
                };

                try
                {
                    var mail = new MailMessage
                    {
                        From = new MailAddress(fromEmail),
                        Subject = emailLog.Subject,
                        Body = emailLog.Body
                    };

                    mail.To.Add(toEmail);

                    if (!string.IsNullOrEmpty(attachmentPath))
                        mail.Attachments.Add(new Attachment(attachmentPath));

                    using var smtp = new SmtpClient(_settings.SmtpHost)
                    {
                        Port = _settings.SmtpPort,
                        Credentials = new NetworkCredential(fromEmail, _settings.SenderPassword),
                        EnableSsl = true
                    };

                    await smtp.SendMailAsync(mail);

                    emailLog.Status = EmailStatus.Enviado;
                    emailLog.Message = "E-mail enviado com sucesso.";
                    await _emailRepository.AddAsync(emailLog);

                    return true;
                }
                catch (Exception ex)
                {
                    emailLog.Status = EmailStatus.Erro;
                    emailLog.Message = ex.Message;
                    await _emailRepository.AddAsync(emailLog);
                }
            }

            return false;
        }
    }
}
