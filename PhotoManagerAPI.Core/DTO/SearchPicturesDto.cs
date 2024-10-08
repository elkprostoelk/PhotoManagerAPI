using PhotoManagerAPI.Core.Enums;

namespace PhotoManagerAPI.Core.DTO
{
    public class SearchPicturesDto
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public string? Iso { get; set; }

        public string? CameraModel { get; set; }

        public bool? Flash { get; set; }

        public float? DelayTimeMilliseconds { get; set; }

        public string? FocusDistance { get; set; }

        public DateTime? ShootingDateFrom { get; set; }

        public DateTime? ShootingDateTo { get; set; }

        public int Page { get; set; } = 1;

        public int ItemsPerPage { get; set; } = 10;

        public string? SortBy { get; set; }

        public SortOrder? SortOrder { get; set; }
    }
}
