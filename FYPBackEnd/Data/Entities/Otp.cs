using System.Xml.Linq;
using System.Xml;
using System;

namespace FYPBackEnd.Data.Entities
{
    public class Otp : BaseEntity
    {
        public string UserId { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string OtpCode { get; set; }
        public bool IsUsed { get; set; }
        public string Purpose { get; set; }
    }
}
