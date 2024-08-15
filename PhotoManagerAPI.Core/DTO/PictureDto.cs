namespace PhotoManagerAPI.Core.DTO
{
    public class PictureDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string PhysicalPath { get; set; } = string.Empty;

        public int Width { get; set; }

        public int Height { get; set; }

        public string? Iso { get; set; }

        public string? CameraModel { get; set; }

        public bool? Flash { get; set; }

        public float? DelayTimeMilliseconds { get; set; }

        public string? FocusDistance { get; set; }

        public DateTime Created { get; set; }

        public DateTime? ShootingDate { get; set; }

        public ShortUserDto Owner { get; set; } = new();
    }
}
