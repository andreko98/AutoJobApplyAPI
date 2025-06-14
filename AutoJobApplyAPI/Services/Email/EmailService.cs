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
        private readonly DataProtectionService _dataProtectionService;

        public EmailService(
            IOptions<EmailSettings> options,
            IEmailRepository emailRepository,
            DataProtectionService dataProtectionService)
        {
            _settings = options.Value;
            _emailRepository = emailRepository;
            _dataProtectionService = dataProtectionService;
        }

        public async Task<bool> SendEmail(int userId, string toCompany, string jobTitle, string message, string attachmentPath)
        {
            try
            {
                // Busca a senha criptografada do banco
                var credential = _emailRepository.GetEmailCredential(userId);
                if (credential == null)
                    throw new Exception("Credencial de e-mail não encontrada.");

                // Descriptografa o email e senha
                var senderEmail = _dataProtectionService.Unprotect(credential.Email);

                var tasks = _settings.EmailPatterns.Select(async pattern =>
                {
                    var toEmail = pattern.Replace("{company}", toCompany.ToLower().Trim());

                    var emailLog = new EmailLog
                    {
                        FromEmail = senderEmail,
                        ToEmail = toEmail,
                        Subject = $"Candidatura para vaga: {jobTitle}",
                        Body = message,
                        AttachmentPath = attachmentPath,
                        SentAt = DateTime.UtcNow
                    };

                    try
                    {
                        using var mail = new MailMessage
                        {
                            From = new MailAddress(senderEmail),
                            Subject = emailLog.Subject,
                            Body = emailLog.Body
                        };

                        mail.To.Add(toEmail);

                        if (!string.IsNullOrEmpty(attachmentPath))
                            mail.Attachments.Add(new Attachment(attachmentPath));

                        await new SmtpClient(_settings.SmtpHost)
                        {
                            Port = _settings.SmtpPort,
                            Credentials = new NetworkCredential(senderEmail, _dataProtectionService.Unprotect(credential.EncryptedPassword)),
                            EnableSsl = true
                        }.SendMailAsync(mail);

                        emailLog.Status = EmailStatus.Enviado;
                        emailLog.Message = "E-mail enviado com sucesso.";
                        await _emailRepository.AddAsync(emailLog);
                    }
                    catch (Exception ex)
                    {
                        emailLog.Status = EmailStatus.Erro;
                        emailLog.Message = ex.Message;
                        await _emailRepository.AddAsync(emailLog);
                    }
                });

                await Task.WhenAll(tasks);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar e-mail: {ex.Message}");
                return false;
            }
        }

        public bool SaveEmailCredential(EmailCredentialRequest request)
        {
            try
            {
                // Criptografa email e senha
                var encryptedEmail = _dataProtectionService.Protect(request.Email);
                var encryptedPassword = _dataProtectionService.Protect(request.Password);

                var credential = new EmailCredential
                {
                    Email = encryptedEmail,
                    EncryptedPassword = encryptedPassword,
                    UserId = request.UserId
                };

                // Salva no repositório
                _emailRepository.SaveEmailCredential(credential);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao cadastrar credencial: {ex.Message}");
                return false;
            }
        }
    }
}

