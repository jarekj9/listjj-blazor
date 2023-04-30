using Listjj.Infrastructure.ViewModels;
using Listjj_frontend.Models;

namespace Listjj_frontend.Data
{
    public class MappingProfiles : AutoMapper.Profile
    {
        public MappingProfiles()
        { 
            CreateMap<ApplicationUser, UserViewModel>();
        }
    }
}
