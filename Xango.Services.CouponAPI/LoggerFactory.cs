namespace Xango.Services.CouponAPI
{
    public class LoggerFactory
    {
        public static ILoggerFactory GetLoggerFactory(WebApplication app)
        {
            var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();

            // Create a logger and log
            var logger = loggerFactory.CreateLogger("Startup");
            logger.LogInformation("Application started");

            return loggerFactory;
        }
    }
}
