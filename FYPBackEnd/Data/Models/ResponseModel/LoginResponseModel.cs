using System;

namespace FYPBackEnd.Data.Models.ResponseModel
{
    public class LoginResponseModel
    {
        public string Email { get; set; }
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public DateTime ExpiresIn { get; set; }

        public bool IsKycComplete { get; set; }
        public bool IsPINSet { get; set; }
        public bool IsPanicPINSet { get; set; }
        
    }
}
