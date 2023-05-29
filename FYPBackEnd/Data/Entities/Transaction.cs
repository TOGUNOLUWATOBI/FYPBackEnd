using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace FYPBackEnd.Data.Entities
{
    public class Transaction : BaseEntity
    {
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public string UserId { get; set; }
        public string ThirdPartyReference { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public string PostingType { get; set; } // cr, dr
        public string TransactionType { get; set; } // Cash_in,transfer, airtime/data, BillsPayment
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceBeforeTransaction { get; set; }
       
        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceAfterTransaction { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TrxAmnt { get; set; }// amount + trxfee
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TrxFee { get; set; }
        public string Status { get; set; }
        public string Beneficiary { get; set; }
        public string BeneficiaryBank { get; set; }
        public string BeneficiaryAccountNumber { get; set; }
        public string BeneficiaryBankCode { get; set; }
    }
}
