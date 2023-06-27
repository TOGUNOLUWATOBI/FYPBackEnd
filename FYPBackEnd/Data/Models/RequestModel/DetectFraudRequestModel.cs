namespace FYPBackEnd.Data.Models.RequestModel
{
    public class DetectFraudRequestModel
    {
        public int step { get; set; }
        public int amount { get; set; }
        public decimal oldbalanceOrg { get; set; }
        public decimal newbalanceOrig { get; set; }
        public int oldbalanceDest { get; set; }
        public int newbalanceDest { get; set; }
        public int isFlaggedFraud { get; set; }
    }


    public class DetectFraudResponseModel
    {
        public int[] predictions { get; set; }

    }
}
