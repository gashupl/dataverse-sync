namespace Pg.DataverseSync.Engine.Target
{
    public interface ITargetDataRepository
    {
        public TargetRecordModificationResult InsertRecord(TargetRecord record);

        public TargetRecordModificationResult UpdateRecord(TargetRecord record);

        public TargetRecordModificationResult DeleteRecord(TargetRecord record);
    }
}
