using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobApplyDatabase.Repositories.ApiKey
{
    public interface IApiKeyRepository
    {
        string? GetApiKey(string provider);
    }
}
