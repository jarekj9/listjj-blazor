using System.Collections.Generic;
using AutoMapper;
using Listjj.Models;
using Listjj.ViewModels;

namespace List.Helpers
{
    public static class MapperHelper
    {
        public static List<Tdst> MapItems<Tsrc, Tdst>(List<Tsrc> source)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<ListjjMappingProfile>();
            });

            IMapper iMapper = config.CreateMapper();
            var destination = iMapper.Map<List<Tsrc>, List<Tdst>>(source);
            return destination;
        }

        public static Tdst MapItem<Tsrc, Tdst>(Tsrc source, Tdst destination)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<ListjjMappingProfile>();   
            });
            IMapper iMapper = config.CreateMapper();
            iMapper.Map<Tsrc, Tdst>(source, destination);
            return destination;
        }

    }
}