using Microsoft.Extensions.Logging;

namespace Pg.DataverseSync.Engine.Application
{
    public abstract class ServiceBase<T>
    {
        protected readonly ILogger logger;

        public ServiceBase(ILogger<T> logger) 
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected void LogIfEnabled(LogLevel logLevel, string message, params object[] args)
        {
            if (logger.IsEnabled(logLevel))
            {
                logger.Log(logLevel, message, args);
            }
        }
    }
}
