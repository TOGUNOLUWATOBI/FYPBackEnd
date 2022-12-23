using System.ComponentModel.DataAnnotations;

namespace FYPBackEnd.Data.Models.RequestModel
{
    public class SignUpRequestModel
    {
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Lastname { get; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string LGA { get; set; }
        public string Address { get; set; }
        public string State { get; set; }

        [Required]
        public string Gender { get; set; }
    }
}
