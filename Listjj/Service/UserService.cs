using Listjj.Abstract;
using Listjj.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Listjj.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUnitOfWork UnitOfWork;
        public UserService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            this.userManager = userManager;
            UnitOfWork = unitOfWork;

        }

        public string GetRole(ApplicationUser user)
        {
            var roles = Task.Run(() => userManager.GetRolesAsync(user)).Result as List<string>;
            return roles.Count > 0 ? roles[0] : "";
        }

        public async Task<ApplicationUser> GetByApiKey(string apiKey)
        {
            Guid.TryParse(apiKey, out var parsedKey);
            var user = await UnitOfWork.Users.GetByApiKey(parsedKey);
            if (user == null || parsedKey == Guid.Empty || user.Id == new Guid())
            {
                return null;
            }
            return user;
        }
    }
}