using AutoJobApplyDatabase.Entities;

namespace AutoJobApplyAPI.Services.Interface
{
    public interface IExternalApiService
    {
        Task<string> GenerateMessageOpenAI(User user, Job job);
    }
}
