using System.Collections.Generic;
using System.Threading.Tasks;
using Listjj.Models;
using Listjj.Abstract;
using Microsoft.AspNetCore.Identity;


namespace Listjj.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> userManager;
        public UserService(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public string GetRole(ApplicationUser user)
        {
            var roles = Task.Run(() => userManager.GetRolesAsync(user)).Result as List<string>;
            return roles.Count > 0 ? roles[0] : "";
        }
    }
}