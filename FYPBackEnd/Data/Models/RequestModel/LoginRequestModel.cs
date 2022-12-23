using System.ComponentModel.DataAnnotations;

namespace FYPBackEnd.Data.Models.RequestModel
{
    public class LoginRequestModel
    {
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
