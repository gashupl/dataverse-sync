using Pg.DataverseSync.Engine.Core.Model;

namespace Pg.DataverseSync.Engine.Application.Synchronization
{
    internal class SyncMetadataExecutionContext
    {
        public List<string> SynchronizedTableNames { get; set; } = new List<string>();

        public List<Table>? SourceTables { get; set; }

        public Dictionary<string, bool> TargetTableExists { get; } =
            new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
    }
}
