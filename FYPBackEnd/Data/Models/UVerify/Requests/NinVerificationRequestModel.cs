namespace FYPBackEnd.Data.Models.UVerify.Requests
{
    public class NinVerificationRequestModel
    {
        public string report_type { get; set; }
        public string type { get; set; }
        public string reference { get; set; }
        public string image { get; set; }
        public bool subject_consent { get; set; }
    }

}
