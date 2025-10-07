using AutoMapper;
using Xango.Services.InventoryApi.Model;
using Xango.Services.Utility;
using Xango.Models.Dto;

namespace Xango.Services.InventoryApi
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDto, Product>().ReverseMap();
            }, (ILoggerFactory)new LoggerFactory());
            return mappingConfig;
        }
    }
}
