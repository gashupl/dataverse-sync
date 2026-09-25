using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Pg.DataverseSync.Engine.Application.Data;
using Pg.DataverseSync.Engine.Target;

namespace Pg.DataverseSync.Engine.Application.ExecutionContext.Handlers
{
    public abstract class ExecutionContextHandlerBase<T> : LoggingServiceBase<T>
    {
        protected readonly ITargetDataRepository targetDataRepository;
        protected readonly ITargetRecordFactory targetRecordFactory;

        public ExecutionContextHandlerBase(ITargetDataRepository targetDataRepository,
            ITargetRecordFactory targetRecordFactory, ILogger<T> logger) : base(logger)
        {
            this.targetDataRepository = targetDataRepository;
            this.targetRecordFactory = targetRecordFactory;
        }

        protected void HandleExecutionResult(TargetRecordModificationResult result, EntityReference entityRef, String messageName)
        {
            if (!result.Success)
            {
                LogIfEnabled(LogLevel.Error,
                    "{MessageName} record failed for entity LogicalName={LogicalName}, Id={Id}. Error: {ErrorMessage}",
                    messageName,
                    entityRef.LogicalName,
                    entityRef.Id,
                    result.Message!);
                throw new InvalidOperationException($"Failed to {messageName} record for entity '{entityRef.LogicalName}' with id '{entityRef.Id}': {result.Message}");
            }

            LogIfEnabled(LogLevel.Information,
                "Record successfully {MessageName}d for entity LogicalName={LogicalName}, Id={Id}",
                messageName,
                entityRef.LogicalName,
                entityRef.Id);
        }
    }
}
