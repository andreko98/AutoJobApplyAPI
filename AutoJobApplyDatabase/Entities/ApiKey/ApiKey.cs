using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobApplyDatabase.Entities
{
    public class ApiKey : IApiKey
    {
        public int Id { get; set; }
        public string Provider { get; set; }
        public string Key { get; set; }
    }
}
