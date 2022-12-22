using System.ComponentModel.DataAnnotations;

namespace FYPBackEnd.Data.Models
{
    public class LoginRequestModel
    {
        [Required]
        public string EmailAddress { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
