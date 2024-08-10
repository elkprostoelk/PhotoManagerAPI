using PhotoManagerAPI.Core.DTO;

namespace PhotoManagerAPI.Core.Services
{
    public interface IPictureUploaderService
    {
        Task<ServiceResultDto<string>> UploadAsync(UploadPictureDto uploadPictureDto);
    }
}
