using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Moq;
using PhotoManagerAPI.Core.DTO;
using PhotoManagerAPI.Core.Services;
using PhotoManagerAPI.DataAccess.Repositories;

namespace PhotoManagerAPI.Tests.Services;

[ExcludeFromCodeCoverage]
public class UserServiceTests : BaseServiceTests
{
    private readonly Mock<ILogger<UserService>> _loggerMock = new();
    private readonly UserService _userService;

    public UserServiceTests()
    {
        var userRepository = new UserRepository(DbContext);
        var roleRepository = new RoleRepository(DbContext);
        _userService = new UserService(userRepository, roleRepository, _loggerMock.Object, Configuration);
    }

    [Fact]
    public async Task CreateNewUserAsync_UserExists_ReturnsUnsuccess()
    {
        // Arrange

        var dto = new NewUserDto
        {
            UserName = "UserName"
        };
        
        // Act

        var result = await _userService.CreateNewUserAsync(dto);

        // Assert
        
        Assert.False(result.IsSuccess);
        Assert.Contains("The user already exists.", result.Errors);
    }

    [Fact]
    public async Task CreateNewUserAsync_RoleNotExists_ReturnsUnsuccess()
    {
        // Arrange

        var dto = new NewUserDto
        {
            UserName = "mike_destro1r",
            Role = "AwesomeMan"
        };
        
        // Act

        var result = await _userService.CreateNewUserAsync(dto);

        // Assert
        
        Assert.False(result.IsSuccess);
        Assert.Contains("The specified role does not exist.", result.Errors);
    }

    [Fact]
    public async Task CreateNewUserAsync_UserCreated_ReturnsSuccess()
    {
        // Arrange

        var dto = new NewUserDto
        {
            UserName = "mike_destro1r",
            Role = "User",
            Email = "dexterm@mail.com",
            Password = "12n9-6El>nG",
            ConfirmPassword = "12n9-6El>nG",
            FullName = "Michael Dexter"
        };
        
        // Act

        var result = await _userService.CreateNewUserAsync(dto);

        // Assert
        
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task SignInAsync_UserNotFound_ReturnsUnsuccess()
    {
        // Arrange

        var loginDto = new LoginDto
        {
            UserNameOrEmail = "someusername",
            Password = "somepassword"
        };

        // Act

        var result = await _userService.SignInAsync(loginDto);

        // Assert

        Assert.False(result.IsSuccess);
        Assert.Contains("User was not found.", result.Errors);
    }

    [Fact]
    public async Task SignInAsync_InvalidPassword_ReturnsUnsuccess()
    {
        // Arrange

        var loginDto = new LoginDto
        {
            UserNameOrEmail = "alexnevr@mail.com",
            Password = "somepassword"
        };

        // Act

        var result = await _userService.SignInAsync(loginDto);

        // Assert

        Assert.False(result.IsSuccess);
        Assert.Contains("Invalid password.", result.Errors);
    }

    [Fact]
    public async Task SignInAsync_ValidCredentials_ReturnsSuccess()
    {
        // Arrange

        var loginDto = new LoginDto
        {
            UserNameOrEmail = "sallynvr1234@mail.com",
            Password = "MyStrongPa$$word3"
        };

        // Act

        var result = await _userService.SignInAsync(loginDto);

        // Assert

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
        Assert.NotNull(result.Container);
    }
}