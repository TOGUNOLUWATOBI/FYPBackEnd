using System;

namespace FYPBackEnd.Data.Models.UVerify.Responses
{
    public class BvnverifyResponseModel
    {
        public bool success { get; set; }
        public int statusCode { get; set; }
        public string message { get; set; }
        public string name { get; set; }
        public Data data { get; set; }
        public object[] links { get; set; }
    }

    public class Data
    {
        public Validations validations { get; set; }
        public string parentId { get; set; }
        public string status { get; set; }
        public bool dataValidation { get; set; }
        public bool selfieValidation { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string image { get; set; }
        public string enrollmentBranch { get; set; }
        public string enrollmentInstitution { get; set; }
        public string mobile { get; set; }
        public string dateOfBirth { get; set; }
        public bool isConsent { get; set; }
        public string idNumber { get; set; }
        public string businessId { get; set; }
        public string type { get; set; }
        public string gender { get; set; }
        public DateTime requestedAt { get; set; }
        public string country { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime lastModifiedAt { get; set; }
        public string id { get; set; }
        public Requestedby requestedBy { get; set; }
    }

    public class Validations
    {
        public Data1 data { get; set; }
        public Selfie selfie { get; set; }
        public string validationMessages { get; set; }
    }

    public class Data1
    {
        public Lastname lastName { get; set; }
        public Dateofbirth dateOfBirth { get; set; }
        public Firstname firstName { get; set; }
    }

    public class Lastname
    {
        public bool validated { get; set; }
        public string value { get; set; }
    }

    public class Dateofbirth
    {
        public bool validated { get; set; }
        public string value { get; set; }
    }

    public class Firstname
    {
        public bool validated { get; set; }
        public string value { get; set; }
    }

    public class Selfie
    {
        public Selfieverification selfieVerification { get; set; }
    }

    public class Selfieverification
    {
        public int confidenceLevel { get; set; }
        public bool match { get; set; }
        public string image { get; set; }
    }

    public class Requestedby
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string middleName { get; set; }
        public string id { get; set; }
    }

}

