using System.ComponentModel.DataAnnotations;

namespace ModelLayer
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string? email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? password { get; set; }
    }
}