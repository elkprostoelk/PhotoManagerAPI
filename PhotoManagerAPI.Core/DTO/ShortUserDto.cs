namespace PhotoManagerAPI.Core.DTO
{
    public class ShortUserDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? FullName { get; set; }
    }
}
