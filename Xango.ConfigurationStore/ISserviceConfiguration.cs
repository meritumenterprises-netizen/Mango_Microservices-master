using Xango.Services.Dto;   

namespace Xango.ConfigurationStore
{
    public interface ISserviceConfiguration
    {
        bool RegisterService(string serviceName, ServiceDto service);
        ServiceDto? GetService(string serviceName);
        List<ServiceDto> GetAllServices();
    }
}
