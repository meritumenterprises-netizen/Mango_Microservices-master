using AutoMapper;
using Xango.Services.Dto;
using Mango.Services.ProductAPI.Models;
using LoggerFactory = Xango.Services.Dto.LoggerFactory;
using Xango.Models.Dto;

namespace Mango.Services.ProductAPI
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
