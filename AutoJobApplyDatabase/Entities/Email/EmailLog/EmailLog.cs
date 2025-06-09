namespace AutoJobApplyDatabase.Entities
{
    public class EmailLog : IEmailLog
    {
        public int Id { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string AttachmentPath { get; set; }
        public DateTime SentAt { get; set; }
        public EmailStatus Status { get; set; }
        public string Message { get; set; }
    }

    public enum EmailStatus
    {
        Erro = 0,
        Enviado = 1
    }
}
