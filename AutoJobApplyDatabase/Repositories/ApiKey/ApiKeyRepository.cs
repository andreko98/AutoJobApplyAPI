using AutoJobApplyDatabase.Context;

namespace AutoJobApplyDatabase.Repositories.ApiKey
{
    public class ApiKeyRepository : IApiKeyRepository
    {
        private readonly AppDbContext _context;

        public ApiKeyRepository(AppDbContext context)
        {
            _context = context;
        }

        public string? GetApiKey(string provider)
        {
            return _context.ExternalApiKeys
                .Where(a => a.Provider == provider)
                .Select(a => a.ApiKey)
                .FirstOrDefault();
        }
    }
}
