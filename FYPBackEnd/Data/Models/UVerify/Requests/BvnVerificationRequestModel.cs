namespace FYPBackEnd.Data.Models.UVerify.Request
{
    public class BvnVerificationRequestModel
    {
        public string id { get; set; }
        public Metadata metadata { get; set; }
        public bool isSubjectConsent { get; set; }
        public Validations validations { get; set; }
        public bool shouldRetrivedNin { get; set; }
    }

    public class Metadata
    {
        public string requestId { get; set; }
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
        public string dateOfBirth { get; set; }
    }

    public class Selfie
    {
        public string image { get; set; }
    }

}
