namespace Pg.DataverseSync.Engine.Target.SqlServer
{
    public interface IDatabaseSchemaRepository
    {
        bool TargetTableExists(string tableName); 
    }
}