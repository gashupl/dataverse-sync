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

        public CreateUserResult CreateUser(User user)
        {
            if (_userRepository.FindByUsername(user.Username) is not null)
            {
                return new CreateUserResult
                {
                    Success = false,
                    ErrorMessage = "Username already exists"
                };
            }

            if (_userRepository.FindByEmail(user.Email) is not null)
            {
                return new CreateUserResult
                {
                    Success = false,
                    ErrorMessage = "Email already exists"
                };
            }

            var userId = _userRepository.CreateUser(user);

            return new CreateUserResult
            {
                Success = true,
                UserId = userId
            };
        }

        public User? GetUserDetailsByUsername(string username)
        {
            return _userRepository.FindByUsername(username);
        }

        public User? GetUserDetailsByEmail(string email)
        {
            return _userRepository.FindByEmail(email);
        }

        public User? GetUserDetailsById(int id)
        {
            return _userRepository.FindById(id);
        }
    }
}
