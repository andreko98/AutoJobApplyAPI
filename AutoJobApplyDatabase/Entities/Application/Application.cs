namespace AutoJobApplyDatabase.Entities
{
    public class Application : IApplication
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int JobId { get; set; }
        public DateTime AppliedAt { get; set; }
        public string MessageSent { get; set; }
        public ApplicationStatus Status { get; set; }

        public virtual User User { get; set; }
        public virtual Job Job { get; set; }
    }

    public enum ApplicationStatus
    {
        Enviado,
        Erro,
        Pendente
    }
}
