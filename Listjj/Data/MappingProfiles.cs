
using System;
using Listjj.Models;
using Listjj.ViewModels;
using System.Linq;
using System.Collections.Generic;

public class ListjjMappingProfile : AutoMapper.Profile
{
    public ListjjMappingProfile()
    {
        CreateMap<ListItem, ListItemViewModel>();
        //.ForMember(d => d.FilesList, s => s.MapFrom(s => s.Files.Select(f => new Tuple<string,Guid>(f.Name,f.Id)).ToList()));

        CreateMap<ListItemViewModel, ListItem>()
            .ForMember(d => d.Files, act => act.Ignore());
        CreateMap<Category, CategoryViewModel>();
        CreateMap<CategoryViewModel, Category>();
        CreateMap<File, FileViewModel>();
        CreateMap<FileViewModel, File>();
    }
}