namespace AutoJobApplyDatabase.Entities
{
    public class EmailCredential : IEmailCredential
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string EncryptedPassword { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
