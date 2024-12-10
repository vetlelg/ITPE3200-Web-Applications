using System.ComponentModel.DataAnnotations;

namespace ITPE3200ExamProject.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Not a valid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A password is required")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
    ErrorMessage = "Password doesn't meet the password complexity requirements.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
