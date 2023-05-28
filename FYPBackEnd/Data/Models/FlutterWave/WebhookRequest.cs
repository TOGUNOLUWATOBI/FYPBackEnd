using System;
using System.Text.Json.Serialization;

namespace FYPBackEnd.Data.Models.FlutterWave
{
    public class WebhookRequest
    {
        [JsonPropertyName("event")]
        public string Event { get; set; }
        [JsonPropertyName("event.type")]
        public string eventtype { get; set; }
        public Data data { get; set; }
    }


    public class Data
    {
        public int id { get; set; }
        public string account_number { get; set; }
        public string bank_name { get; set; }
        public string bank_code { get; set; }
        public string fullname { get; set; }
        public DateTime created_at { get; set; }
        public string currency { get; set; }
        public string debit_currency { get; set; }
        public int amount { get; set; }
        public float fee { get; set; }
        public string status { get; set; }
        public string reference { get; set; }
        public object meta { get; set; }
        public string narration { get; set; }
        public object approver { get; set; }
        public string complete_message { get; set; }
        public int requires_approval { get; set; }
        public int is_approved { get; set; }
 
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string callback_url { get; set; }
        public int AccountId { get; set; }
        public Bvn_Data bvn_data { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public object deletedAt { get; set; }
    }

    public class Bvn_Data
    {
        public object additionalInfo1 { get; set; }
        public object branchName { get; set; }
        public string dateOfBirth { get; set; }
        public string email { get; set; }
        public object enrollBankCode { get; set; }
        public string enrollUserName { get; set; }
        public object enrollmentDate { get; set; }
        public string faceImage { get; set; }
        public string firstName { get; set; }
        public string gender { get; set; }
        public object landmarks { get; set; }
        public object lgaOfCapture { get; set; }
        public string lgaOfOrigin { get; set; }
        public string lgaOfResidence { get; set; }
        public string maritalStatus { get; set; }
        public string middleName { get; set; }
        public object nameOnCard { get; set; }
        public string nin { get; set; }
        public string phoneNumber1 { get; set; }
        public object phoneNumber2 { get; set; }
        public string productReference { get; set; }
        public object serialNo { get; set; }
        public string stateOfCapture { get; set; }
        public string stateOfOrigin { get; set; }
        public string stateOfResidence { get; set; }
        public string surname { get; set; }
        public string watchlisted { get; set; }
    }

}
