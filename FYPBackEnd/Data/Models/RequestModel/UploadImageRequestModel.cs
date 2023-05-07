using Microsoft.AspNetCore.Http;
using RestSharp;

namespace FYPBackEnd.Data.Models.RequestModel
{
    public class UploadImageRequestModel
    {
        public HttpFile file { get; set; }
    }
}
