namespace Pg.DataverseSync.Engine.Target
{
    public interface ITargetDataRepository
    {

        public TargetRecordModificationResult InsertRecord(string tableName, TargetRecord record);

        public TargetRecordModificationResult UpdateRecord(string tableName, TargetRecord record, TargetRecord primaryKeyRecord);

        public TargetRecordModificationResult DeleteRecord(string tableName, TargetRecord primaryKeyRecord);
    }
}
