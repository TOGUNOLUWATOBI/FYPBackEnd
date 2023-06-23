using Microsoft.AspNetCore.Identity;
using System;
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
        public int SaltProperty { get; set; }
        public string TransactionPIN { get; set; }
        public string PanicPIN { get; set; }
        public int PinTries { get; set; }
        public bool IsPINSet { get; set; }
        public bool IsPanicPINSet { get; set; }
        public string State { get; set; }
        public string Gender { get; set; }
        public string Status { get; set; }

        public string ProficePictureId { get; set; }

        public bool IsKYCComplete { get; set; }
        public bool IsAndroidDevice { get; set; }
        public string DeviceToken { get; set; }

        public string FCMToken { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
