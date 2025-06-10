using AutoJobApplyAPI.Models;
using AutoJobApplyAPI.Services.Interface;
using AutoJobApplyDatabase.Entities;
using AutoJobApplyDatabase.Repositories;
using Microsoft.IdentityModel.Tokens;

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

        public async Task<User> CreateUserAsync(UserCreateRequest request)
        {
            try
            {
                var user = new User
                {
                    Name = request.Name,
                    LastName = request.LastName,
                    Email = request.Email,
                    Password = _dataProtectionService.Protect(request.Password),
                    DateOfBirth = request.DateOfBirth,
                    Address = request.Address,
                    About = request.About,
                    CvPath = string.Empty,
                    EmailCredentialId = null
                };

                return await _userRepository.AddAsync(user);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> UpdateUserAsync(int id, UserCreateRequest request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            user.Name = request.Name;
            user.LastName = request.LastName;
            user.Email = request.Email;

            if (!request.Password.IsNullOrEmpty())
            user.Password = _dataProtectionService.Protect(request.Password);

            user.DateOfBirth = request.DateOfBirth;
            user.Address = request.Address;
            user.About = request.About;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<string?> UploadCVAsync(int userId, IFormFile file)
        {
            if (file.ContentType != "application/pdf")
                return null;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            // Apaga o arquivo anterior, se existir
            if (!string.IsNullOrEmpty(user.CvPath))
            {
                var oldFullPath = Path.Combine("Uploads", user.CvPath);
                if (File.Exists(oldFullPath))
                {
                    File.Delete(oldFullPath);
                }
                user.CvPath = string.Empty; // Limpa o campo no banco antes de atualizar
                await _userRepository.UpdateAsync(user);
            }

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

        public async Task<string?> GetCVPathAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            return user.CvPath;
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
