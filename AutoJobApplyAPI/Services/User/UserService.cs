using AutoJobApplyAPI.Services.Interface;
using AutoJobApplyDatabase.Entities;
using AutoJobApplyDatabase.Repositories;

namespace AutoJobApplyAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            return await _userRepository.AddAsync(user);
        }

        public async Task<bool> UpdateUserAsync(int id, User updatedUser)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            user.Name = updatedUser.Name;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.DateOfBirth = updatedUser.DateOfBirth;
            user.Address = updatedUser.Address;
            user.About = updatedUser.About;

            if (!string.IsNullOrEmpty(updatedUser.CvPath))
                user.CvPath = updatedUser.CvPath;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<string?> UploadCVAsync(int id, IFormFile file)
        {
            if (file.ContentType != "application/pdf")
                return null;

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            var folder = Path.Combine("Uploads", "CV");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var relativePath = Path.Combine("CV", fileName);
            var fullPath = Path.Combine("Uploads", relativePath);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            user.CvPath = relativePath;
            await _userRepository.UpdateAsync(user);

            return relativePath;
        }
    }
}
