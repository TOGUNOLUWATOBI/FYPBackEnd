namespace FYPBackEnd.Data.Entities
{
    public class Beneficiary : BaseEntity
    {
        // todo: create beneficiary and implement it for both mobile and backend
        public string Name { get; set; }
        public string VerifiedName { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string NetworkProvide { get; set; }
        public string PhoneNumber { get; set; }

    }
}
