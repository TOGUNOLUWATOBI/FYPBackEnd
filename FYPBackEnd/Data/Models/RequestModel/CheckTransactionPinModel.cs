namespace FYPBackEnd.Data.Models.RequestModel
{
    public class CheckTransactionPinModel
    {
        public string TrxPin { get; set; }
    }

    public class AddTransactionPinModel
    {
        public string TrxPin { get; set; }
    }

    public class ChangeTransactionPinModel
    {
        public string OldTrxPin { get; set;}
        public string NewTrxPin { get; set;}

    }

    public class AddPanicModePinModel
    {
        public string PanicPin { get; set; }
    }

    public class ChangePanicModePinModel
    {
        public string OldPanicPin { get; set; }
        public string NewPanicPin { get; set; }

    }
}
