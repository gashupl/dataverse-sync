namespace Pg.DataverseSync.Domain.Services
{
    public interface IEndpointStepCreationService
    {
        EndpointStepCreationResult CreateStepForEntity(string entityName);
    }
}
