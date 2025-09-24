using AutoMapper;

namespace Xango.Services.AuthAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
            });
            return mappingConfig;
        }
    }
}
