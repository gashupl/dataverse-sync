using Pg.DataverseSync.Api.Application.Repositories;
using Pg.DataverseSync.Api.Domain;
using Pg.DataverseSync.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore; 

namespace Pg.DataverseSync.Api.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;

        public UserRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CreateUser(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user.Id;
        }

        public async Task<User?> FindByIdAsync(int id)
        {
            return await _dbContext.Users.SingleOrDefaultAsync(user => user.Id == id);
        }

        public async Task<User?> FindByUsernameAsync(string username)
        {
            return await _dbContext.Users.SingleOrDefaultAsync(user => user.Username == username);
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            return await _dbContext.Users.SingleOrDefaultAsync(user => user.Email == email);
        }
    }
}
