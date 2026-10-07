using System.ComponentModel.DataAnnotations;

namespace ModelLayer
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Reset token is required")]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [StringLength(
            20,
            MinimumLength = 8,
            ErrorMessage = "Password must be between 8 and 20 characters"
        )]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$",
            ErrorMessage = "Password must contain uppercase, lowercase, number and special character"
        )]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare(
            "NewPassword",
            ErrorMessage = "New password and confirm password do not match"
        )]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}