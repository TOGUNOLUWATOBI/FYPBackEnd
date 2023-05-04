namespace FYPBackEnd.Data.Models.UVerify.Requests
{
    public class DriverLicesnseVerificationRequestModel
    {
        public string id { get; set; }
        public Metadata metadata { get; set; }
        public bool isSubjectConsent { get; set; }
        public Validations validations { get; set; }
    }
}
