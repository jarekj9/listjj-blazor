using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace Listjj.Models
{
    public class ApplicationUserRole : IdentityRole<Guid>
    {
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationRole Role { get; set; }
  }
}