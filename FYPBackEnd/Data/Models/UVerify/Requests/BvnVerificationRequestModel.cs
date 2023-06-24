using FYPBackEnd.Data.Models.UVerify.Requests;

namespace FYPBackEnd.Data.Models.UVerify.Request
{
    public class BvnVerificationRequestModel
    {
        public string id { get; set; }
        
        public bool isSubjectConsent { get; set; }
        public Validations validations { get; set; }
        public bool shouldRetrivedNin { get; set; }
    }
}
