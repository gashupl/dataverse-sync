using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Pg.DataverseSync.Engine.Application;
using Pg.DataverseSync.Engine.Target;

namespace Pg.DataverseSync.Engine.Functions;

public class SchemaSynchronizationFunction : LoggingServiceBase<SchemaSynchronizationFunction>
{
    private readonly ISourceMetadataService _sourceMetadataService;

    public SchemaSynchronizationFunction(
        ISourceMetadataService sourceMetadataService,
        ILogger<SchemaSynchronizationFunction> logger) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(sourceMetadataService);
        ArgumentNullException.ThrowIfNull(logger);

        _sourceMetadataService = sourceMetadataService;
    }

    [Function(nameof(SchemaSynchronizationFunction))]
    public void Run(
#if DEBUG
        [TimerTrigger("%SchemaSyncSchedule%", RunOnStartup = true)] TimerInfo timer)
#else
        [TimerTrigger("%SchemaSyncSchedule%")] TimerInfo timer)
#endif
    {
        LogIfEnabled(LogLevel.Information, "SchemaSynchronizationFunction triggered at: {UtcNow}", DateTime.UtcNow);

        var tables = _sourceMetadataService.GetTables();

        if (tables is null || tables.Count == 0)
        {
            LogIfEnabled(LogLevel.Warning, "No sync tables found. Schema synchronization skipped.");
            return;
        }

        LogIfEnabled(LogLevel.Information, "Starting schema synchronization for {Count} table(s).", tables.Count);

        foreach (var table in tables)
        {
            LogIfEnabled(LogLevel.Information, "Upserting schema for table: {TableName}", table.Name);
        }

        LogIfEnabled(LogLevel.Information, "Schema synchronization completed.");
    }
}
