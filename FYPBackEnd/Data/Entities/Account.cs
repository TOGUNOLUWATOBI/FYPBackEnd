using System;

namespace FYPBackEnd.Data.Entities
{
    public class Account : BaseEntity
    {
        public string UserId { get; set; }
        public string AccountNumber { get; set; }
        public string ThirdPartyAccountNumber { get; set; }
        public string ThirdPartyReference { get; set; }
        public string ThirdPartyBankName { get; set; }
        public string ThirdPartyBankCode { get; set; }
        public decimal Balance { get; set; }
        public decimal ThirdPartyBalance { get; set; }
        public string Status { get; set; }
        public string AccountName { get; set; }
        
    }
}
