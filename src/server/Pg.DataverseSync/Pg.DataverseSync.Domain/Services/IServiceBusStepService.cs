namespace Pg.DataverseSync.Domain.Services
{
    public interface IServiceBusStepService
    {
        ServiceOperationResult CreateStepForEntity(string entityName, string messageName);

        ServiceOperationResult DeleteStepForEntity(string entityName, string messageName);
    }
}
