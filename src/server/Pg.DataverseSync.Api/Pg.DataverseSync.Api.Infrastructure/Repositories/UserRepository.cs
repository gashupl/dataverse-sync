using Pg.DataverseSync.Api.Application.Repositories;
using Pg.DataverseSync.Api.Domain;
using Pg.DataverseSync.Api.Infrastructure.Data;

namespace Pg.DataverseSync.Api.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public int CreateUser(User user)
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            return user.Id;
        }

        public User? FindById(int id)
        {
            return _dbContext.Users.SingleOrDefault(user => user.Id == id);
        }

        public User? FindByUsername(string username)
        {
            return _dbContext.Users.SingleOrDefault(user => user.Username == username);
        }

        public User? FindByEmail(string email)
        {
            return _dbContext.Users.SingleOrDefault(user => user.Email == email);
        }
    }
}
