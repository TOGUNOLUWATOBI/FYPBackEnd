namespace FYPBackEnd.Data.Models.UVerify.Responses
{
    public class NinVerifyResponseModel
    {
        public string message { get; set; }
        public DataNin data { get; set; }
        public int status_code { get; set; }
    }

    public class DataNin
    {
        public string id { get; set; }
        public string type { get; set; }
        public string task_created_by { get; set; }
        public string reference_id { get; set; }
        public string status { get; set; }
        public Response response { get; set; }
    }

    public class Response
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string middle_name { get; set; }
        public string dob { get; set; }
        public string mobile { get; set; }
        public string reference_id { get; set; }
        public object batch_id { get; set; }
        public string birth_country { get; set; }
        public string birth_lga { get; set; }
        public string birth_state { get; set; }
        public object card_status { get; set; }
        public object central_id { get; set; }
        public object document_no { get; set; }
        public object educational_level { get; set; }
        public object employment_status { get; set; }
        public string gender { get; set; }
        public object height { get; set; }
        public object maiden_name { get; set; }
        public object marital_status { get; set; }
        public string nok_address1 { get; set; }
        public string nok_address2 { get; set; }
        public string nok_firstname { get; set; }
        public string nok_lga { get; set; }
        public string nok_state { get; set; }
        public string nok_surname { get; set; }
        public string nok_town { get; set; }
        public string ospokenlang { get; set; }
        public string photo { get; set; }
        public string religion { get; set; }
        public string residence_address_line1 { get; set; }
        public object residence_town { get; set; }
        public string residence_lga { get; set; }
        public string residence_state { get; set; }
        public string self_origin_lga { get; set; }
        public string self_origin_place { get; set; }
        public string self_origin_state { get; set; }
        public string signature { get; set; }
        public object tracking_id { get; set; }
    }

}
