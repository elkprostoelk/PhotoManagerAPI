using PhotoManagerAPI.Core.DTO;

namespace PhotoManagerAPI.Core.Services;

public interface IPictureService
{
    Task<ServiceResultDto> AddAsync(NewPictureDto newPictureDto);
}