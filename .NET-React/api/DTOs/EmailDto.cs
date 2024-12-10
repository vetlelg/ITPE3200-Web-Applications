using System.ComponentModel.DataAnnotations;

namespace ITPE3200ExamProject.DTOs;

public class EmailDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Not a valid email address")]
    public string Email { get; set; } = string.Empty;
}
