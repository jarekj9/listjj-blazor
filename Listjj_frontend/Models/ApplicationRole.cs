using Microsoft.AspNetCore.Identity;

namespace Listjj_frontend.Models
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
