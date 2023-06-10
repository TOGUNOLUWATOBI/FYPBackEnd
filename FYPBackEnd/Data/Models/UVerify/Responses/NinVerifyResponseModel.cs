using System;

namespace FYPBackEnd.Data.Models.UVerify.Responses
{
    public class NinVerifyResponseModel
    {
        public bool success { get; set; }
        public int statusCode { get; set; }
        public string message { get; set; }
        public NinData data { get; set; }
        public object[] links { get; set; }
    }

    public class NinData
    {
        public string id { get; set; }
        public NinValidations validations { get; set; }
        public object parentId { get; set; }
        public string status { get; set; }
        public object reason { get; set; }
        public bool dataValidation { get; set; }
        public bool selfieValidation { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string image { get; set; }
        public string signature { get; set; }
        public object mobile { get; set; }
        public string birthState { get; set; }
        public string birthLGA { get; set; }
        public string birthCountry { get; set; }
        public string dateOfBirth { get; set; }
        public bool isConsent { get; set; }
        public string idNumber { get; set; }
        public string businessId { get; set; }
        public string type { get; set; }
        public bool allValidationPassed { get; set; }
        public DateTime requestedAt { get; set; }
        public string requestedById { get; set; }
        public string country { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime lastModifiedAt { get; set; }
        public Metadata metadata { get; set; }
        public Requestedby requestedBy { get; set; }
    }

    public class NinValidations
    {
        public NinData1 data { get; set; }
        public NinSelfie selfie { get; set; }
        public string validationMessages { get; set; }
    }

    public class NinData1
    {
        public NinLastname lastName { get; set; }
        public NinDateofbirth dateOfBirth { get; set; }
        public NinFirstname firstName { get; set; }
    }

    public class NinLastname
    {
        public bool validated { get; set; }
        public string value { get; set; }
    }

    public class NinDateofbirth
    {
        public bool validated { get; set; }
        public string value { get; set; }
    }

    public class NinFirstname
    {
        public bool validated { get; set; }
        public string value { get; set; }
    }

    public class NinSelfie
    {
        public NinSelfieverification selfieVerification { get; set; }
    }

    public class NinSelfieverification
    {
        public int confidenceLevel { get; set; }
        public int threshold { get; set; }
        public bool match { get; set; }
        public string image { get; set; }
    }

    public class Metadata
    {
    }

    public class NinRequestedby
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string middleName { get; set; }
        public string id { get; set; }
    }

}
