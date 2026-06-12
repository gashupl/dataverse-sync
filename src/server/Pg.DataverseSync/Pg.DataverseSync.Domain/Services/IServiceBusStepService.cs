namespace Pg.DataverseSync.Domain.Services
{
    public interface IServiceBusStepService
    {
        ServiceOperationResult CreateStepsForEntity(string entityName, string[] messageNames);

        ServiceOperationResult DeleteStepsForEntity(string entityName, string[] messageNames);
    }
}
