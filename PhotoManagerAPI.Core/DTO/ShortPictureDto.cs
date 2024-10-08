namespace PhotoManagerAPI.Core.DTO
{
    public class ShortPictureDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string PhysicalPath { get; set; } = string.Empty;

        public int Width { get; set; }

        public int Height { get; set; }

        public ShortUserDto Owner { get; set; } = new();
    }
}
