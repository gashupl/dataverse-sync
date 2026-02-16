namespace Pg.DataverseSync.Engine.Domain
{
    public interface ISourceMetadataService
    {
        List<string> GetTablesNames();
    }
}
