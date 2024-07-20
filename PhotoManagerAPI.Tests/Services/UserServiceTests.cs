using Microsoft.Extensions.Logging;
using Moq;
using PhotoManagerAPI.Core.DTO;
using PhotoManagerAPI.Core.Services;
using PhotoManagerAPI.DataAccess.Repositories;

namespace PhotoManagerAPI.Tests.Services;

public class UserServiceTests : BaseServiceTests
{
    private readonly Mock<ILogger<UserService>> _loggerMock = new();
    private readonly UserService _userService;

    public UserServiceTests()
    {
        var userRepository = new UserRepository(DbContext);
        var roleRepository = new RoleRepository(DbContext);
        _userService = new UserService(userRepository, roleRepository, _loggerMock.Object);
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
}