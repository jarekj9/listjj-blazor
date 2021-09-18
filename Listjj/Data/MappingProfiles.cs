
using System;
using Listjj.Models;
using Listjj.ViewModels;
using System.Linq;
using System.Collections.Generic;

public class ListjjMappingProfile : AutoMapper.Profile
{
    public ListjjMappingProfile()
    {
        CreateMap<ListItem, ListItemViewModel>()
        .ForMember(dest => dest.Tags, opt => opt.MapFrom((src, dest) => src.Tags.Split(',').ToList()));
        //.ForMember(d => d.FilesList, s => s.MapFrom(s => s.Files.Select(f => new Tuple<string,Guid>(f.Name,f.Id)).ToList()));

        CreateMap<ListItemViewModel, ListItem>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom((src, dest) => String.Join(',', src.Tags.Where(x => !string.IsNullOrWhiteSpace(x)))))
            .ForMember(d => d.Files, act => act.Ignore());
        CreateMap<Category, CategoryViewModel>();
        CreateMap<CategoryViewModel, Category>();
        CreateMap<File, FileViewModel>();
        CreateMap<FileViewModel, File>();
    }
}