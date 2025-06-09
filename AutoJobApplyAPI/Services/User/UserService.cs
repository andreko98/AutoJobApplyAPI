using AutoJobApplyAPI.Services.Interface;
using AutoJobApplyDatabase.Entities;
using AutoJobApplyDatabase.Repositories;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;

namespace AutoJobApplyAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly DataProtectionService _dataProtectionService;

        public UserService(IUserRepository userRepository,
            DataProtectionService dataProtectionService)
        {
            _userRepository = userRepository;
            _dataProtectionService = dataProtectionService;
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
            user.Password = _dataProtectionService.Protect(user.Password);

            return await _userRepository.AddAsync(user);
        }

        public async Task<bool> UpdateUserAsync(int id, User updatedUser)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            user.Name = updatedUser.Name;
            user.LastName = updatedUser.LastName;
            user.Email = updatedUser.Email;
            user.Password = _dataProtectionService.Protect(updatedUser.Password);
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

        public async Task<bool> ValidatePasswordAsync(int userId, string password)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || string.IsNullOrEmpty(user.Password))
                return false;

            try
            {
                // Descriptografa a senha armazenada
                var decryptedPassword = _dataProtectionService.Unprotect(user.Password);
                // Compara com a senha informada
                return decryptedPassword == password;
            }
            catch
            {
                // Caso a descriptografia falhe, considera inválido
                return false;
            }
        }
    }
}
