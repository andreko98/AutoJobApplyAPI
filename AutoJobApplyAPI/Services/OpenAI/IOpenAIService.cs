using AutoJobApplyDatabase.Entities;

namespace AutoJobApplyAPI.Services.Interface
{
    public interface IOpenAIService
    {
        Task<string> GenerateMessage(User user, Job job);
    }
}
