namespace FYPBackEnd.Data.Models.FlutterWave
{
    public class VirtualAccount
    {
    }

    public class CreateVIrtualRequestModel
    {
        public string Email { get; set; }
        public bool is_permanent { get; set; }
        public string Bvn { get; set; }
        public string tx_ref { get; set; }
        public string Phonenumber { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Narration { get; set; }
    }


    public class CreateVIrtualResponseModel
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public VirtualAccountData Data { get; set; }
    }

    public class VirtualAccountData
    {
        public string response_code { get; set; }
        public string response_message { get; set; }
        public string flw_ref { get; set; }
        public string order_ref { get; set; }
        public string account_number { get; set; }
        public string frequency { get; set; }
        public string bank_name { get; set; }
        public string created_at { get; set; }
        public string expiry_date { get; set; }
        public string note { get; set; }
        public object amount { get; set; }
    }


    public class GetVirtualAccountResponseModel
    {
        public string status { get; set; }
        public string message { get; set; }
        public VirtualAccountData data { get; set; }
    }
}
