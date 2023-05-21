namespace FYPBackEnd.Data.Models.RequestModel
{
    public class BuyAirtimeRequestModel
    {
        public string Country { get; set; }
        public string Customer { get; set; } // this is the phone number you want to buy the airtime or data for
        public string Type { get; set; }
        public int Amount { get; set; }
        public decimal TrxAmount { get; set; }
        public string Recurrence { get; set; }
        public string Reference { get; set; }
        public string longitude { get; set; }
        public string latitude { get; set; }
    }
}
