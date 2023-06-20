namespace FYPBackEnd.Data.Models.UVerify.Requests
{
    public class NinVerificationRequestModel
    {
        public string id { get; set; }
        public bool isSubjectConsent { get; set; }
        public Validations validations { get; set; }
    }

    public class Validations
    {
        public Data data { get; set; }
        public Selfie selfie { get; set; }
    }

    public class Data
    {
        public string lastName { get; set; }
        public string firstName { get; set; }
    }

    public class Selfie
    {
        public string image { get; set; }
    }


}
