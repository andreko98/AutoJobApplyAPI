using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobApplyDatabase.Entities
{
    public class User : IUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string About { get; set; }
        public string CvPath { get; set; }
        public int EmailCredentialId { get; set; }

        public virtual EmailCredential EmailCredential { get; set; }
    }
}
