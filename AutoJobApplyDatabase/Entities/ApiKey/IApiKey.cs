namespace AutoJobApplyDatabase.Entities
{
    public interface IExternalApiKey
    {
        int Id { get; set; }
        string Provider { get; set; }
        string ApiKey { get; set; }
    }
}
