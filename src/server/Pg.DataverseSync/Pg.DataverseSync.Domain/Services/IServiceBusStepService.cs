namespace Pg.DataverseSync.Domain.Services
{
    public interface IServiceBusStepService
    {
        ServiceOperationResult CreateStepForEntity(string entityName, string messageName);
    }
}
