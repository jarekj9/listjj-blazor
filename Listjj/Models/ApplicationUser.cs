using Microsoft.AspNetCore.Identity;
using System;

namespace Listjj.Models
{
  public class ApplicationUser : IdentityUser
  {
    public Guid ApiKey { get; set; }
  }
}