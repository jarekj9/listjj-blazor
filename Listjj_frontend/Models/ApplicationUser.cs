using Microsoft.AspNetCore.Identity;

namespace Listjj_frontend.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public Guid ApiKey { get; set; }
    }
}
