using System.ComponentModel.DataAnnotations;

namespace ModelLayer
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; } = string.Empty;
    }
}