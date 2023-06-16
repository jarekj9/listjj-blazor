using Microsoft.AspNetCore.Identity;
using System;

namespace Listjj.Models
{
  public class ApplicationUser : IdentityUser<Guid>
  {
        public Guid ApiKey { get; set; }
  }
}