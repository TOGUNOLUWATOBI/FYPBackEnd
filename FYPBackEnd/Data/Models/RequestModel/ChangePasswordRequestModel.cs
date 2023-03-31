using System.ComponentModel.DataAnnotations;

namespace FYPBackEnd.Data.Models.RequestModel
{
    public class ChangePasswordRequestModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
