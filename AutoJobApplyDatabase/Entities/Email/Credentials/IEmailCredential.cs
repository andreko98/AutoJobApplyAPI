namespace AutoJobApplyDatabase.Entities
{
    public interface IEmailCredential
    {
        int Id { get; set; }
        string Email { get; set; }
        string EncryptedPassword { get; set; }
        public int UserId { get; set; }

        User User { get; set; }
    }
}
