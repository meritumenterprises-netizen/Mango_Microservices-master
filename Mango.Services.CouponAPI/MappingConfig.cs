using AutoMapper;
using Xango.Services.Dto;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using LoggerFactory = Xango.Services.Dto.LoggerFactory;

namespace Mango.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CouponDto, Coupon>();
                config.CreateMap<Coupon, CouponDto>();
            }, (ILoggerFactory)new LoggerFactory());
            return mappingConfig;
        }
    }
}
