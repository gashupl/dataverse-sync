namespace Pg.DataverseSync.Domain.Repositories
{
    public interface IEnvironmentVariablesRepository
    {
        string GetValue(string name); 
    }
}
