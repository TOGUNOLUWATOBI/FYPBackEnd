namespace FYPBackEnd.Data.Enums
{
    public enum TransactionType
    {
        Transfer,
        Airtime,
        BillsPayment,
        CashOut
    }

    public enum TransactionStatus
    {
        Successful,
        Pending,
        Failed
    }

    public enum PostingType
    {
        Cr,
        Dr
    }

   
}
