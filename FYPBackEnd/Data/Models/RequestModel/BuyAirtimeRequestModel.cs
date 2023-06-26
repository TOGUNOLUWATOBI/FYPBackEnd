namespace FYPBackEnd.Data.Models.RequestModel
{
    public class BuyAirtimeRequestModel
    {
        
        public string Customer { get; set; } // this is the phone number you want to buy the airtime or data for
        public string Type { get; set; }
        public int Amount { get; set; }
        public decimal TrxAmount { get; set; }
        public decimal TransferAmount { get; set; }

        public string longitude { get; set; }
        public string latitude { get; set; }
    }
}
