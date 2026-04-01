using Pg.DataverseSync.Api.Application.Results;
using Pg.DataverseSync.Api.Domain;

namespace Pg.DataverseSync.Api.Application.Services.Interfaces
{
    public interface IUserService
    {
        public CreateUserResult CreateUser(User user); 
    }
}
