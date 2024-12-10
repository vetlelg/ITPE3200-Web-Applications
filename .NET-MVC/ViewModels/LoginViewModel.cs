using System.ComponentModel.DataAnnotations;

namespace ITPE3200ExamProject.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Not a valid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
