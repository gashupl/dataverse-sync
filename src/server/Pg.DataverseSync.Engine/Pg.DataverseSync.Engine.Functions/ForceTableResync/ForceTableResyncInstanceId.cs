namespace Pg.DataverseSync.Engine.Functions.ForceTableResync;

public static class ForceTableResyncInstanceId
{
    public static string For(string tableName)
    {
        return $"force-table-resync-{tableName.Trim().ToLowerInvariant()}";
    }
}
