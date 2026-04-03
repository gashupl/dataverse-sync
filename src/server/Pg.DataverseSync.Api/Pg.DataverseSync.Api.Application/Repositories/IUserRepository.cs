using Pg.DataverseSync.Api.Domain;

namespace Pg.DataverseSync.Api.Application.Repositories
{
    public interface IUserRepository
    {
        public Task<int> CreateUser(User user);
        public Task<User?> FindByUsernameAsync(string username);
        public Task<User?> FindByEmailAsync(string email);
        public Task<User?> FindByIdAsync(int id);
    }
}
