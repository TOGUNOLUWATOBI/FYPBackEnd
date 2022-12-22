using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace FYPBackEnd.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Lga { get; set; }
        public string State { get; set; }
        public string Gender { get; set; }

        public string Status { get; set; }
    }
}
