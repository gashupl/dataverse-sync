using Pg.DataverseSync.Api.Application.Results;
using Pg.DataverseSync.Api.Domain;

namespace Pg.DataverseSync.Api.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<CreateUserResult> CreateUser(User user); 

        Task<User?> GetUserDetailsByIdAsync(int id);

        Task<User?> GetUserDetailsByUsernameAsync(string username);     
        Task<User?> GetUserDetailsByEmailAsync(string email);
    }
}
