using FYPBackEnd.Data.Entities;

namespace FYPBackEnd.Data.Models.RequestModel
{
    public class TransferRequestModel
    {
        public int Amount { get; set; }
        public decimal TrxAmount { get; set; }
        public string Description { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiaryAccountNumber { get; set; }
        public string BeneficiaryBank { get; set; }
        public string BeneficiaryBankCode { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
    }

    public class VerifyAccountUserRequestModel
    {
        public string account_number { get; set; }
        public string bank_name { get; set; }
    }
}
