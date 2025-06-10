using AutoJobApplyDatabase.Context;
using AutoJobApplyDatabase.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoJobApplyDatabase.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user != null)
            user.Password = string.Empty;

            return user;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<User> AddAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                user.Password = string.Empty;
                return user;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
