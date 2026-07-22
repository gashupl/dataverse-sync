using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Pg.DataverseSync.Engine.Application
{
    [ExcludeFromCodeCoverage]
    public abstract class LoggingServiceBase<T>
    {
        private readonly ILogger<T> _logger;

        protected LoggingServiceBase(ILogger<T> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected void LogIfEnabled(LogLevel logLevel, string message, params object[] args)
        {
            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel, message, args);
            }
        }

        protected void LogIfEnabled(LogLevel logLevel, Exception exception, string message, params object[] args)
        {
            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel, exception, message, args);
            }
        }
    }
}
