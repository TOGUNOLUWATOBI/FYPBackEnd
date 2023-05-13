namespace FYPBackEnd.Data.Models.DTO
{
    public class AccountDto
    {
        public string AccountNumber { get; set; }
        public string ThirdPartyAccountNumber { get; set; }
        public string ThirdPartyBankName { get; set; }
        public decimal Balance { get; set; }
        public string Status { get; set; }
        public string AccountName { get; set; }
    }
}
