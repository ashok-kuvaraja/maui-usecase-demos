using Microsoft.Extensions.Logging;

namespace SalesOrderTracker.Services.Logging
{
    public static class LoggingConfig
    {
        public static void AddAppLogging(this ILoggingBuilder builder)
        {
            // Default filters
            builder.AddFilter("Microsoft", LogLevel.Warning);
            builder.AddFilter("System", LogLevel.Warning);
            builder.AddFilter("SalesOrderTracker", LogLevel.Debug);

    #if DEBUG
            builder.AddDebug();
    #endif
        }
    }
}
