namespace FYPBackEnd.Data.Models.DTO
{
    public class TransactionDto
    {
        public decimal Amount { get; set; }
        public string Reference { get; set; }
        public string BeneficiaryName { get; set; }
        public string BeneficiaryBank { get; set; }

        public string PhoneNumber { get; set; }
    }
}
