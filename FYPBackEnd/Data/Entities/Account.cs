using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FYPBackEnd.Data.Entities
{
    public class Account : BaseEntity
    {
        public string UserId { get; set; }
        public string AccountNumber { get; set; }
        public string ThirdPartyAccountNumber { get; set; }
        public string ThirdPartyReference { get; set; }
        public string ThirdPartyBankName { get; set; }
        public int Tier { get; set; }
        public int Limit { get; set; }
        public string ThirdPartyBankCode { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal ThirdPartyBalance { get; set; }
        public string Status { get; set; }
        public string AccountName { get; set; }
        
    }
}
