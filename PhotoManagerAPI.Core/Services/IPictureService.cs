using PhotoManagerAPI.Core.DTO;

namespace PhotoManagerAPI.Core.Services;

public interface IPictureService
{
    Task<ServiceResultDto> AddAsync(NewPictureDto newPictureDto);
    Task<PictureDto?> GetAsync(Guid id);
    Task<PagedResultDto<ShortPictureDto>> SearchAsync(
        SearchPicturesDto searchPicturesDto,
        CancellationToken cancellationToken);
    Task<ServiceResultDto> DeletePictureAsync(Guid id, Guid userId);
}