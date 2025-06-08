namespace AutoJobApplyAPI.Services
{
    using Microsoft.AspNetCore.DataProtection;

    public class DataProtectionService
    {
        private readonly IDataProtector _protector;

        public DataProtectionService(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("EmailPasswordProtector");
        }

        public string Protect(string input) => _protector.Protect(input);
        public string Unprotect(string input) => _protector.Unprotect(input);
    }
}
