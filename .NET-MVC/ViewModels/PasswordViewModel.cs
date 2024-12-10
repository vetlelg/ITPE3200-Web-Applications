using System.ComponentModel.DataAnnotations;

namespace ITPE3200ExamProject.ViewModels;

public class PasswordViewModel
{

    [Required(ErrorMessage = "A password is required")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
    ErrorMessage = "Password doesn't meet the password complexity requirements.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password confirmation is required")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Old password is required")]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; } = string.Empty;
}
