using AutoMapper;
using Xango.Services.ShoppingCartAPI.Models;
using Xango.Services.Utility;
using Xango.Models.Dto;

namespace Xango.Services.ShoppingCartAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
                config.CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
            }, (ILoggerFactory)new LoggerFactory());
            return mappingConfig;
        }
    }
}
