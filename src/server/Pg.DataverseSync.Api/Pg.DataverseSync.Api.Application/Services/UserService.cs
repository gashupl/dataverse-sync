using Pg.DataverseSync.Api.Application.Repositories;
using Pg.DataverseSync.Api.Application.Results;
using Pg.DataverseSync.Api.Application.Services.Interfaces;
using Pg.DataverseSync.Api.Domain;
using System.Security.Cryptography;

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
            //TODO: Add error handling 
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

        public (byte[] salt, byte[] hash) CreatePasswordHash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                600_000,
                HashAlgorithmName.SHA256,
                32);
            return (salt, hash);
        }
    }
}
