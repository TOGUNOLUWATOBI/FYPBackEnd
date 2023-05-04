using System;

namespace FYPBackEnd.Data.Models.UVerify.Responses
{
    public class DriversLicenseVerifyResponseModel
    {
        public bool success { get; set; }
        public int statusCode { get; set; }
        public string name { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
        public object[] links { get; set; }
    }
}
