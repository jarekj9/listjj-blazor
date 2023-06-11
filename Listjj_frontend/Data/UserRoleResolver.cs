using AutoMapper;
using Listjj.Infrastructure.ViewModels;
using Listjj_frontend.Models;
using Listjj_frontend.Services.Abstract;

namespace Listjj_frontend.Data
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
