using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Listjj.Models
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
  }
}