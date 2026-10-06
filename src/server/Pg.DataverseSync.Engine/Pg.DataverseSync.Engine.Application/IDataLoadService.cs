using Pg.DataverseSync.Engine.Target;

namespace Pg.DataverseSync.Engine.Application
{
    public interface IDataLoadService
    {
        public List<TargetRecordModificationResult> LoadInitialData(string tableName);
    }
}
