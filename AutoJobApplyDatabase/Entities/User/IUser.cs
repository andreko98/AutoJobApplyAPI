namespace AutoJobApplyDatabase.Entities
{
    public interface IUser
    {
        int Id { get; set; }
        string Name { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        DateTime DateOfBirth { get; set; }
        string Address { get; set; }
        string About { get; set; }
        string CvPath { get; set; }
    }
}
