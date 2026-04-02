using Pg.DataverseSync.Api.Application.Results;
using Pg.DataverseSync.Api.Domain;

namespace Pg.DataverseSync.Api.Application.Services.Interfaces
{
    public interface IUserService
    {
        CreateUserResult CreateUser(User user);

        (byte[] salt, byte[] hash) CreatePasswordHash(string password); 
    }
}
