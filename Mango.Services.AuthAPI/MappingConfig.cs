using AutoMapper;
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
            }, (ILoggerFactory)new Microsoft.Extensions.Logging.LoggerFactory());
            return mappingConfig;
        }
    }
}
