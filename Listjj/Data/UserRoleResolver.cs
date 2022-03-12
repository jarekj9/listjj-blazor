using Listjj.Models;
using Listjj.ViewModels;
using AutoMapper;
using Listjj.Service;
using Listjj.Abstract;

namespace Listjj.Data
{
    public class UserRoleResolver : IValueResolver<ApplicationUser, UserViewModel, string>
    {

        private readonly IUserService userService;

        public UserRoleResolver(IUserService userService)
        {
            this.userService = userService;
        }

        public string Resolve(ApplicationUser source, UserViewModel destination, string destMember, ResolutionContext context)
            => userService.GetRole(source);
    }
}
