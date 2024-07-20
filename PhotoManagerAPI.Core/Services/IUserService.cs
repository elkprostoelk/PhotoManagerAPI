using PhotoManagerAPI.Core.DTO;

namespace PhotoManagerAPI.Core.Services;

public interface IUserService
{
    Task<ServiceResultDto> CreateNewUserAsync(NewUserDto newUserDto);
}