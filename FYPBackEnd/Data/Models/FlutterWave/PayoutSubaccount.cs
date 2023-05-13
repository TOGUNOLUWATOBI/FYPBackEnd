using System;

namespace FYPBackEnd.Data.Models.FlutterWave
{
    public class PayoutSubaccount
    {
    }

    public class CreatePaymentSubaccountRequestModel
    {
        public string account_name { get; set; }
        public string email { get; set; }
        public string mobilenumber { get; set; }
        public string country { get; set; }
        public string account_reference { get; set; }
        public string bank_code { get; set; }
    }


    public class CreatePaymentSubaccountResponseModel
    {
        public string status { get; set; }
        public string message { get; set; }
        public SubaccountData data { get; set; }
    }

    public class SubaccountData
    {
        public int id { get; set; }
        public string account_reference { get; set; }
        public string account_name { get; set; }
        public string barter_id { get; set; }
        public string email { get; set; }
        public string mobilenumber { get; set; }
        public string country { get; set; }
        public string nuban { get; set; }
        public string bank_name { get; set; }
        public string bank_code { get; set; }
        public string status { get; set; }
        public DateTime created_at { get; set; }
    }

}
