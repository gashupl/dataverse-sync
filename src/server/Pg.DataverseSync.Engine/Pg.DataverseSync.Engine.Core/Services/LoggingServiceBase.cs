using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Pg.DataverseSync.Engine.Application
{
    [ExcludeFromCodeCoverage]
    public abstract class LoggingServiceBase<T>
    {
        protected readonly ILogger<T> logger;

        protected LoggingServiceBase(ILogger<T> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected void LogIfEnabled(LogLevel logLevel, string message, params object[] args)
        {
            if (logger.IsEnabled(logLevel))
            {
                logger.Log(logLevel, (Exception?)null, message, args);
            }
        }

        protected void LogIfEnabled(LogLevel logLevel, Exception exception, string message, params object[] args)
        {
            if (logger.IsEnabled(logLevel))
            {
                logger.Log(logLevel, exception, message, args);
            }
        }
    }
}
