namespace FYPBackEnd.Data.Entities
{
    public class Transaction : BaseEntity
    {
        public decimal Amount { get; set; }
        public string UserId { get; set; }
        public string ThirdPartyReference { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public string PostingType { get; set; } // cr, dr
        public string TransactionType { get; set; } // transfer, airtime/data, BillsPayment
        public decimal BalanceBeforeTransaction { get; set; }
        public decimal BalanceAfterTransaction { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
        public decimal TrxAmnt { get; set; }// amount + trxfee
        public decimal TrxFee { get; set; }
        public string Status { get; set; }
        public string Beneficiary { get; set; }
        public string BeneficiaryBankCode { get; set; }
    }
}
