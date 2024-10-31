using Microsoft.Extensions.Logging;
using PhotoManagerAPI.Core.DTO;

namespace PhotoManagerAPI.Core.Services
{
    public class PictureUploaderService : IPictureUploaderService
    {
        private readonly ILogger<PictureUploaderService> _logger;

        public PictureUploaderService(
            ILogger<PictureUploaderService> logger)
        {
            _logger = logger;
        }

        public async Task<ServiceResultDto<string>> UploadAsync(UploadPictureDto uploadPictureDto)
        {
            var result = new ServiceResultDto<string>();

            _logger.LogInformation(
                "Attempt to upload a file {FileName} by user {UserId}",
                uploadPictureDto.FileName,
                uploadPictureDto.UserId);

            try
            {
                var todayDate = DateOnly.FromDateTime(DateTime.Now);

                var newFileName = $"{Ulid.NewUlid()}{uploadPictureDto.GetExtension()}";
                var directoryPath = $"{Directory.GetCurrentDirectory()}/img/{todayDate.Year}/{todayDate.Month}/{todayDate.Day}";
                var newFilePath = $"{directoryPath}/{newFileName}";

                if (!Directory.Exists(directoryPath))
                {
                    _logger.LogInformation("Creating a directory {DirectoryPath}", directoryPath);
                    Directory.CreateDirectory(directoryPath);
                }

                if (!File.Exists(newFilePath))
                {
                    _logger.LogInformation(
                        "Started uploading a file {FileName} as {NewFileName} by user {UserId}",
                        uploadPictureDto.FileName,
                        newFileName,
                        uploadPictureDto.UserId);
                    await File.WriteAllBytesAsync(newFilePath, uploadPictureDto.File);
                    _logger.LogInformation(
                        "Finished uploading a file {FileName} to a path {NewFilePath} by user {UserId}",
                        uploadPictureDto.FileName,
                        newFilePath,
                        uploadPictureDto.UserId);
                    result.Container = newFilePath[newFilePath.IndexOf("img", StringComparison.Ordinal)..];
                }
                else
                {
                    _logger.LogWarning(
                        "Failed to upload. A file {NewFileName} already exists. User: {UserId}",
                        newFileName,
                        uploadPictureDto.UserId);
                    result.IsSuccess = false;
                    result.Errors.Add("A file already exists.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "Failed to upload a file {FileName} for user {UserId}.",
                    uploadPictureDto.FileName,
                    uploadPictureDto.UserId);
                result.IsSuccess = false;
                result.Errors.Add("Failed to upload a file. See logs for details.");
            }

            return result;
        }
    }
}
