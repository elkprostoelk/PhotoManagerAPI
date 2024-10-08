namespace PhotoManagerAPI.Core.DTO
{
    public class PagedResultDto<T>
    {
        public List<T> PageItems { get; set; } = [];

        public int TotalItems { get; set; }

        public int Page { get; set; }

        public int PageCount { get; set; }

        public int ItemsPerPage { get; set; }
    }
}
