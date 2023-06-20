using Microsoft.AspNetCore.Http;

namespace FYPBackEnd.Data.Models.RequestModel
{
    public class VerifyKycRequestModel
    {
        public string Email { get; set; }
        public string DocType { get; set; }
        public string DocNumber { get; set; }
        public IFormFile Selfie { get; set; }

    }
}
