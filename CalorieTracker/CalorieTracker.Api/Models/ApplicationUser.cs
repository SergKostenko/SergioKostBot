using Microsoft.AspNetCore.Identity;

namespace CalorieTracker.Api.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}