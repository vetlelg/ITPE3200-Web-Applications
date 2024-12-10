using System.ComponentModel.DataAnnotations;

namespace ITPE3200ExamProject.ViewModels;

public class EmailViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Not a valid email address")]
    public string Email { get; set; } = string.Empty;
}
