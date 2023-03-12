using System;
using Listjj.Models;
using Listjj.ViewModels;
using System.Linq;

namespace Listjj.Data
{
    public class ListjjMappingProfile : AutoMapper.Profile
    {
        public ListjjMappingProfile()
        {
            CreateMap<ListItem, ListItemViewModel>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom((src, dest) => src.Tags.Split(',').ToList()));

            CreateMap<ListItemViewModel, ListItem>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom((src, dest) => String.Join(',', src.Tags.Where(x => !string.IsNullOrWhiteSpace(x)))))
                .ForMember(d => d.Files, act => act.Ignore());

            CreateMap<Category, CategoryViewModel>();
            CreateMap<CategoryViewModel, Category>();

            CreateMap<File, FileViewModel>();
            CreateMap<FileViewModel, File>();

            CreateMap<ApplicationUser, UserViewModel>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom<UserRoleResolver>());
            CreateMap<UserViewModel, ApplicationUser>();



            CreateMap<Listjj.Infrastructure.ViewModels.CategoryViewModel, Category>();
            CreateMap<Category, Listjj.Infrastructure.ViewModels.CategoryViewModel>();

            CreateMap<Listjj.Infrastructure.ViewModels.ListItemViewModel, ListItem>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom((src, dest) => String.Join(',', src.Tags.Where(x => !string.IsNullOrWhiteSpace(x)))))
                .ForMember(d => d.Files, act => act.Ignore());
            CreateMap<ListItem, Listjj.Infrastructure.ViewModels.ListItemViewModel>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom((src, dest) => src.Tags?.Split(',').ToList()));
        }
    }
}
