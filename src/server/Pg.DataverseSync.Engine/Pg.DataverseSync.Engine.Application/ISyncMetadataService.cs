namespace Pg.DataverseSync.Engine.Application
{
    public interface ISyncMetadataService
    {
        SyncMetadataResult Execute();

        SyncMetadataResult Execute(string tableName);
    }
}
