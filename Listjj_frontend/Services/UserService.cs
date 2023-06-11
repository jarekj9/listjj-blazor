using Listjj_frontend.Models;
using Listjj_frontend.Services.Abstract;
using Microsoft.AspNetCore.Identity;


namespace Listjj_frontend.Services
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