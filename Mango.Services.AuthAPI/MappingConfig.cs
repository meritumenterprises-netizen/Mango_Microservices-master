using AutoMapper;
using Mango.Services.AuthAPI.Models;
using Xango.Models.Dto;

namespace Xango.Services.AuthAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<LoginRequestDto, UserDto>();
                config.CreateMap<UserDto, ApplicationUser>().ReverseMap();
            }, (ILoggerFactory)new Microsoft.Extensions.Logging.LoggerFactory());
            return mappingConfig;
        }
    }
}
