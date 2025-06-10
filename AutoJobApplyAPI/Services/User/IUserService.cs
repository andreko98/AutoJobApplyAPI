using AutoJobApplyAPI.Models;
using AutoJobApplyDatabase.Entities;

namespace AutoJobApplyAPI.Services.Interface
{
    public interface IUserService
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateUserAsync(UserCreateRequest user);
        Task<bool> UpdateUserAsync(int id, UserCreateRequest updatedUser);
        Task<string?> UploadCVAsync(int userId, IFormFile file);
        Task<string?> GetCVPathAsync(int userId);
        Task<bool> ValidatePasswordAsync(int userId, string password);
    }
}
