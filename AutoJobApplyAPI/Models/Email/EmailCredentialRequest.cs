namespace AutoJobApplyAPI.Models
{
    public class EmailCredentialRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int UserId { get; set; }
    }
}