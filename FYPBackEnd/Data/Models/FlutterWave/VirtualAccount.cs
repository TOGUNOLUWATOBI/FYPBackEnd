using System;
using System.Collections.Generic;

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



    public class PayoutSubaccountBalance
    {
        public string status { get; set; }
        public string message { get; set; }
        public PaymentSubaccountBalanceData data { get; set; }
    }

    public class PaymentSubaccountBalanceData
    {
        public string currency { get; set; }
        public float available_balance { get; set; }
        public int ledger_balance { get; set; }
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

    public class GetBillCategoriesResponseModel
    {
        public string status { get; set; }
        public string message { get; set; }
        public List<BillCategories> data { get; set; }
    }

    public class BillCategories
    {
        public int id { get; set; }
        public string biller_code { get; set; }
        public string name { get; set; }
        public float default_commission { get; set; }
        public DateTime date_added { get; set; }
        public string country { get; set; }
        public bool is_airtime { get; set; }
        public string biller_name { get; set; }
        public string item_code { get; set; }
        public string short_name { get; set; }
        public int fee { get; set; }
        public bool commission_on_fee { get; set; }
        public string label_name { get; set; }
        public int amount { get; set; }
    }

    public class PayBillResponseModel
    {
        public string status { get; set; }
        public string message { get; set; }
        public PayBill data { get; set; }
    }

    public class PayBill
    {
        public string currency { get; set; }
        public string customer_id { get; set; }
        public string frequency { get; set; }
        public string amount { get; set; }
        public string product { get; set; }
        public string product_name { get; set; }
        public int commission { get; set; }
        public DateTime transaction_date { get; set; }
        public string country { get; set; }
        public string tx_ref { get; set; }
        public object extra { get; set; }
        public string product_details { get; set; }
        public string status { get; set; }
        public string code { get; set; }
    }


    public class PayBillRequestModel
    {
        public string country { get; set; }
        public string customer { get; set; }
        public int amount { get; set; }
        public string recurrence { get; set; }
        public string type { get; set; }
        public string reference { get; set; }
    }



    public class ValidateBillPaymentResponseModel
    {
        public string message { get; set; }
        public string status { get; set; }
        public PayBillData data { get; set; }
    }


    public class PayBillData
    {
        public string response_code { get; set; }
        public object address { get; set; }
        public string response_message { get; set; }
        public string name { get; set; }
        public string biller_code { get; set; }
        public string customer { get; set; }
        public string product_code { get; set; }
        public object email { get; set; }
        public int fee { get; set; }
        public int maximum { get; set; }
        public int minimum { get; set; }
    }


    public class ValidateBillRequestModel
    {
        public string item_code { get; set; }
        public string code { get; set; }
        public string customer { get; set; }
    }

}
