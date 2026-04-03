using Pg.DataverseSync.Api.Application.Repositories;
using Pg.DataverseSync.Api.Application.Results;
using Pg.DataverseSync.Api.Application.Services.Interfaces;
using Pg.DataverseSync.Api.Domain;

namespace Pg.DataverseSync.Api.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<CreateUserResult> CreateUser(User user)
        {
            if (_userRepository.FindByUsernameAsync(user.Username) is not null)
            {
                return new CreateUserResult
                {
                    Success = false,
                    ErrorMessage = "Username already exists"
                };
            }

            if (await _userRepository.FindByEmailAsync(user.Email) is not null)
            {
                return new CreateUserResult
                {
                    Success = false,
                    ErrorMessage = "Email already exists"
                };
            }

            var userId = await _userRepository.CreateUser(user);

            return new CreateUserResult
            {
                Success = true,
                UserId = userId
            };
        }

        public async Task<User?> GetUserDetailsByUsernameAsync(string username)
        {
            return await _userRepository.FindByUsernameAsync(username);
        }

        public async Task<User?> GetUserDetailsByEmailAsync(string email)
        {
            return await _userRepository.FindByEmailAsync(email);
        }

        public async Task<User?> GetUserDetailsByIdAsync(int id)
        {
            return await _userRepository.FindByIdAsync(id);
        }
    }
}
