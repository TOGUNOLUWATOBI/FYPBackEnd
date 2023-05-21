using System;

namespace FYPBackEnd.Data.Models.FlutterWave
{
    public class Transfer
    {
    }


    public class InitiateTransferRequestModel
    {
        public string Account_bank { get; set; }
        public string Account_number { get; set; }
        public int Amount { get; set; }
        public string Narration { get; set; }
        public string Currency { get; set; }
        public string Reference { get; set; }
        public string Callback_url { get; set; }
        public string Debit_currency { get; set; }
        public string Debit_subaccount { get; set; }
    }

    public class InitiateTransferResponseModel
    {
        public string status { get; set; }
        public string message { get; set; }
        public InitiateTransferData data { get; set; }
    }


    public class InitiateTransferData
    {
        public int id { get; set; }
        public string account_number { get; set; }
        public string bank_code { get; set; }
        public string full_name { get; set; }
        public DateTime created_at { get; set; }
        public string currency { get; set; }
        public string debit_currency { get; set; }
        public int amount { get; set; }
        public float fee { get; set; }
        public string status { get; set; }
        public string reference { get; set; }
        public object meta { get; set; }
        public string narration { get; set; }
        public string complete_message { get; set; }
        public int requires_approval { get; set; }
        public int is_approved { get; set; }
        public string bank_name { get; set; }
    }



    public class BankResponseModel
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public BankData[] Data { get; set; }
    }

    public class BankData
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class AccountNameVerificationModel
    {
        public string account_number { get; set; }
        public string account_bank { get; set; }
    }



    public class AccountNameVerificationResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public AccountNameData data { get; set; }
    }

    public class AccountNameData
    {
        public string account_number { get; set; }
        public string account_name { get; set; }
    }



}
