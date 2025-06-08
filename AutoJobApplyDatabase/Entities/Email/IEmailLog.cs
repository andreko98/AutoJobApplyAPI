namespace AutoJobApplyDatabase.Entities
{
    public interface IEmailLog
    {
        int Id { get; set; }
        string FromEmail { get; set; }
        string ToEmail { get; set; }
        string Subject { get; set; }
        string Body { get; set; }
        string AttachmentPath { get; set; }
        DateTime SentAt { get; set; }
        EmailStatus Status { get; set; }
        string Message { get; set; }
    }
}
