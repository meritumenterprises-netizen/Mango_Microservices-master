using AutoMapper;
using Xango.Models.Dto;
using Xango.Services.Dto;
using Mango.Services.OrderAPI.Models;
using System.Runtime.CompilerServices;

namespace Mango.Services.OrderAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<OrderHeaderDto, CartHeaderDto>()
                    .ForMember(dest => dest.CartTotal, u => u.MapFrom(src => src.OrderTotal))
                    .ReverseMap();

                config.CreateMap<CartDetailsDto, OrderDetailsDto>()
                    .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
                    .ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price));

                config.CreateMap<OrderDetailsDto, CartDetailsDto>();

                config.CreateMap<OrderHeader, OrderHeaderDto>().ReverseMap();
                config.CreateMap<OrderDetailsDto, OrderDetails>().ReverseMap();
            }, (ILoggerFactory)new Microsoft.Extensions.Logging.LoggerFactory());

            return mappingConfig;
        }
    }
}
