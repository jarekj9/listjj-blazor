using System;
using Listjj.Models;
using System.Linq;
using Listjj.Infrastructure.ViewModels;

namespace Listjj.Data
{
    public class ListjjMappingProfile : AutoMapper.Profile
    {
        public ListjjMappingProfile()
        {
            CreateMap<ApplicationUser, UserViewModel>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom<UserRoleResolver>());
            CreateMap<UserViewModel, ApplicationUser>();
            CreateMap<ApplicationRole, RoleViewModel>();

            CreateMap<ListItem, ListItemViewModel>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom((src, dest) => src.Tags.Split(',').ToList()));

            CreateMap<ListItemViewModel, ListItem>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom((src, dest) => String.Join(',', src.Tags.Where(x => !string.IsNullOrWhiteSpace(x)))))
                .ForMember(d => d.Files, act => act.Ignore());

            CreateMap<Category, CategoryViewModel>();
            CreateMap<CategoryViewModel, Category>();

            CreateMap<File, FileViewModel>()
                .ForMember(dest => dest.B64Bytes, opt => opt.MapFrom((src, dst) => Convert.ToBase64String(src.Bytes)));
            CreateMap<File, FileSimpleViewModel>();
            CreateMap<FileViewModel, File>();

            CreateMap<CategoryViewModel, Category>();
            CreateMap<Category, CategoryViewModel>();

            CreateMap<ListItemViewModel, ListItem>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom((src, dest) => String.Join(',', src.Tags.Where(x => !string.IsNullOrWhiteSpace(x)))))
                .ForMember(d => d.Files, act => act.Ignore());
            CreateMap<ListItem, ListItemViewModel>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom((src, dest) => src.Tags?.Split(',').ToList()));
        }
    }
}
