using Pg.DataverseSync.Api.Domain;

namespace Pg.DataverseSync.Api.Application.Repositories
{
    public interface IUserRepository
    {
        public int CreateUser(User user);
        public User? FindByUsername(string username);
        public User? FindByEmail(string email);
    }
}
