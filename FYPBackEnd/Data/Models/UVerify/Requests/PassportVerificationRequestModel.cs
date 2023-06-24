namespace FYPBackEnd.Data.Models.UVerify.Requests
{
    public class PassportVerificationRequestModel
    {
        public string id { get; set; }
        public bool isSubjectConsent { get; set; }
        public Validations validations { get; set; }
    }

}
