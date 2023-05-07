using Microsoft.AspNetCore.Http;

namespace FYPBackEnd.Data.Models.RequestModel
{
    public class UploadImageRequestModel
    {
        public IFormFile File { get; set; }
    }
}
