namespace PhotoManagerAPI.Core.Configurations
{
    public class ImageOptions
    {
        public int MaxFileSizeBytes { get; set; }

        public List<string> AllowedFileTypes { get; set; } = [];
    }
}
