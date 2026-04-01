using Pg.DataverseSync.Api.Domain;

namespace Pg.DataverseSync.Api.Application.Repositories
{
    public interface IUserRepository
    {
        public void CreateUser(User user);
    }
}
