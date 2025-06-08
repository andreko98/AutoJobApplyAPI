using AutoJobApplyDatabase.Entities;

namespace AutoJobApplyAPI.Services.Interface
{
    public interface IUserService
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);
        Task<bool> UpdateUserAsync(int id, User updatedUser);
        Task<string?> UploadCVAsync(int id, IFormFile file);
    }
}
