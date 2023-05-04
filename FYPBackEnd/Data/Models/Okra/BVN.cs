using System;

namespace FYPBackEnd.Data.Models.Okra
{
    public class BVN
    {
    }


    public class GetBvnByIdRequestModel
    {
        public string id { get; set; }
    }

    public class GetBvnByBvnRequestModel
    {
        public string bvn { get; set; }
    }


    public class GetBVNResponseModel
    {
        public string status { get; set; }
        public string message { get; set; }
        public Identity data { get; set; }
    }

    public class Identity
    {
        public string id { get; set; }
        public string firstname { get; set; }
        public string middlename { get; set; }
        public string lastname { get; set; }
        public string fullname { get; set; }
        public string dob { get; set; }
        public string bvn { get; set; }
        public string gender { get; set; }
        public Customer customer { get; set; }
        public string verification_country { get; set; }
        public string env { get; set; }
        public DateTime created_at { get; set; }
        public object[] aliases { get; set; }
        public string[] phone { get; set; }
        public object[] email { get; set; }
        public string[] address { get; set; }
        public string nationality { get; set; }
        public string lga_of_origin { get; set; }
        public string lga_of_residence { get; set; }
        public string state_of_origin { get; set; }
        public string state_of_residence { get; set; }
        public string marital_status { get; set; }
        public object[] next_of_kins { get; set; }
        public object nin { get; set; }
        public Photo_Id[] photo_id { get; set; }
        public Enrollment enrollment { get; set; }
    }

    public class Customer
    {
        public string _id { get; set; }
        public string name { get; set; }
    }

    public class Enrollment
    {
        public string bank { get; set; }
        public string branch { get; set; }
        public string registration_date { get; set; }
    }

    public class Photo_Id
    {
        public string url { get; set; }
        public string image_type { get; set; }
    }


}
