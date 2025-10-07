using Microsoft.Extensions.Logging;

namespace Xango.Services.Utility;

public class MyLoggingProviderFactory
{
    private readonly ILoggerFactory _loggerFactory;

    public MyLoggingProviderFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public ILogger CreateLogger(string categoryName)
    {
        // You can add custom logic here if needed
        return _loggerFactory.CreateLogger(categoryName);
    }
}
