namespace PhotoManagerAPI.Core.DTO
{
    public class UploadPictureDto
    {
        public byte[] File { get; set; } = [];

        public string FileName { get; set; } = string.Empty;

        public Guid UserId { get; set; }

        public string GetExtension() => Path.GetExtension(FileName) ?? string.Empty;
    }
}
