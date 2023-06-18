using AutoMapper;
using Listjj.Infrastructure.ViewModels;
using Listjj.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;

namespace Listjj.APIs
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> logger;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;

        public UsersController(UserManager<ApplicationUser> userManager, ILogger<UsersController> logger, IMapper mapper)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.userManager = userManager;
        }


        [Route("api/[controller]/all")]
        [HttpGet]
        public async Task<JsonResult> GetAllUsers()
        {
            var users = userManager.Users.ToList();
            var userVms = mapper.Map<List<UserViewModel>>(users);
            return Json(userVms);
        }

        [Route("api/[controller]/addorupdate")]
        [HttpPost]
        public async Task<JsonResult> AddorUpdateUser([FromBody] UserViewModel userVm)
        {
            bool result = false;
            var user = await userManager.FindByIdAsync(userVm.Id.ToString());
            if (user == null)
            {
                var newUser = mapper.Map<ApplicationUser>(userVm);
                var addResult = await userManager.CreateAsync(newUser);
                return new JsonResult(addResult.Succeeded);
            }

            mapper.Map<UserViewModel, ApplicationUser>(userVm, user);
            result = (await userManager.UpdateAsync(user)).Succeeded;
            result = await SetUsersPassword(user, userVm.Password) & result ? result : false;
            result = await SetUsersRole(user, userVm.Role) & result ? result : false;
            result = (await userManager.SetUserNameAsync(user, userVm.Email)).Succeeded & result ? result : false;

            return new JsonResult(result);
        }

        [Route("api/[controller]/delete")]
        [HttpPost]
        public async Task<JsonResult> DeleteUser([FromBody] UserViewModel userVm)
        {
            var user = await userManager.FindByIdAsync(userVm.Id.ToString());
            var result = (await userManager.DeleteAsync(user)).Succeeded;
            return new JsonResult(result);
        }

        private async Task<bool> SetUsersPassword(ApplicationUser user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return true;
            }
            await userManager.RemovePasswordAsync(user);
            var result = (await userManager.AddPasswordAsync(user, password)).Succeeded;

            return result;
        }

        private async Task<bool> SetUsersRole(ApplicationUser user, string role)
        {
            var assignedRoles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, assignedRoles);
            if (string.IsNullOrEmpty(role))
            {
                return true;
            }
            var result = (await userManager.AddToRoleAsync(user, role)).Succeeded;

            return result;
        }

    }
}
