using Microsoft.AspNetCore.Identity;

namespace Xango.Services.AuthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
