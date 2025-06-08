namespace AutoJobApplyDatabase.Entities
{
    public interface IApiKey
    {
        int Id { get; set; }
        string Provider { get; set; }
        string Key { get; set; }
    }
}
