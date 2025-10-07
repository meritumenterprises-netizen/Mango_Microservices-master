using AutoMapper;
using Xango.Services.ProductAPI.Models;
using Xango.Services.Utility;
using Xango.Models.Dto;

namespace Xango.Services.ProductAPI
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
