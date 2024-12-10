using Microsoft.AspNetCore.Identity;

namespace ITPE3200ExamProject.Models
{
    // Inherits everything from IdentityUser
    // Add more properties if needed
    public class Account : IdentityUser
    {
        // AccountId is inherited from IdentityUser and is a string guid
    }
}
