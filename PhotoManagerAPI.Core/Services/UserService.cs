using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Logging;
using PhotoManagerAPI.Core.DTO;
using PhotoManagerAPI.DataAccess.Entities;
using PhotoManagerAPI.DataAccess.Repositories;

namespace PhotoManagerAPI.Core.Services;

public class UserService : IUserService
{
    private const int SaltByteSize = 128 / 8;
    private const int HashByteSize = 256 / 8;
    private const int IterationCount = 100_000;
    
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _logger = logger;
    }
    
    public async Task<ServiceResultDto> CreateNewUserAsync(NewUserDto newUserDto)
    {
        var userExists = await _userRepository.ExistsAsync(newUserDto.UserName);

        if (userExists)
        {
            _logger.LogInformation("The user {UserName} already exists. Unable to create a duplicate user account.", newUserDto.UserName);
            return new ServiceResultDto
            {
                IsSuccess = false,
                Errors = ["The user already exists."]
            };
        }

        var roleId = await _roleRepository.GetAsync(newUserDto.Role);
        if (!roleId.HasValue)
        {
            _logger.LogWarning("The role {Role} does not exist. Unable to create a user account without a valid role.", newUserDto.Role);
            return new ServiceResultDto
            {
                IsSuccess = false,
                Errors = ["The specified role does not exist."]
            };
        }

        var (salt, passwordHash) = HashPassword(newUserDto.Password);

        var user = new User
        {
            Id = Ulid.NewUlid().ToGuid(),
            Name = newUserDto.UserName,
            FullName = newUserDto.FullName,
            RoleId = roleId.Value,
            Salt = salt,
            PasswordHash = passwordHash,
            CreationDate = DateTime.UtcNow
        };

        var created = await _userRepository.CreateAsync(user);

        return new ServiceResultDto
        {
            IsSuccess = created,
            Errors = created ? [] : ["User has not been created. Check logs for details."]
        };
    }

    private static (string salt, string hash) HashPassword(string password, string? userSalt = null)
    {
        if (string.IsNullOrWhiteSpace(password))
            return (string.Empty, string.Empty);

        var salt = string.IsNullOrWhiteSpace(userSalt)
            ? RandomNumberGenerator.GetBytes(SaltByteSize)
            : Convert.FromBase64String(userSalt);

        var passwordHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: IterationCount,
            numBytesRequested: HashByteSize));

        return (Convert.ToBase64String(salt), passwordHash);
    }
}