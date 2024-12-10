using System.ComponentModel.DataAnnotations;

namespace ITPE3200ExamProject.DTOs;

public class PasswordDto
{
    [Required(ErrorMessage = "A password is required")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
    ErrorMessage = "Password doesn't meet the password complexity requirements.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Old password is required")]
    [DataType(DataType.Password)]
    public string OldPassword { get; set; } = string.Empty;
}
