using System;

namespace FYPBackEnd.Data.Models.DTO
{
    public class TransactionDto
    {
        public decimal Amount { get; set; }
        public string Referebce { get; set; }
        public string Id { get; set; }
        public DateTime? CreationDate { get; set; }
        public string PostingType { get; set; } // cr, dr
        public string TransactionType { get; set; } // transfer, airtime/data, BillsPayment
        public string Status { get; set; }
        public string Beneficiary { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiaryAccountNumber { get; set; }
        public string BeneficiaryBank { get; set; }
        public string PhoneNumber { get; set; }
    }
}
