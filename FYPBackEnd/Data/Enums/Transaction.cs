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
        Completed,
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
