using AutoMapper;

namespace Xango.Web.Mapping
{
    public interface ILoggingProviderFactory
    {
        ILogger CreateLogger(string categoryName);
    }

    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
            }, (ILoggerFactory)new LoggerFactory());
            return mappingConfig;
        }
    }
}
