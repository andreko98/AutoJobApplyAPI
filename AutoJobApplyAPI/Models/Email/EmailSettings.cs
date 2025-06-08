namespace AutoJobApplyAPI.Models
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SenderPassword { get; set; }
        public List<string> EmailPatterns { get; set; }
    }
}
