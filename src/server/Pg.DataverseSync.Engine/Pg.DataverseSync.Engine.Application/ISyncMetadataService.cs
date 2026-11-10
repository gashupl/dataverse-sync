namespace Pg.DataverseSync.Engine.Application
{
    public interface ISyncMetadataService
    {
        SyncMetadataResult Execute();

        SyncMetadataResult Execute(string tableName);

        PurgeTableResult PurgeTable(string tableName); 
    }
}
