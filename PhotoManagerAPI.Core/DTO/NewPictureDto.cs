namespace PhotoManagerAPI.Core.DTO;

public class NewPictureDto
{
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }

    public byte[] File { get; set; } = [];
    
    public int Width { get; set; }
    
    public int Height { get; set; }
    
    public string? Iso { get; set; }
    
    public string? CameraModel { get; set; }
    
    public bool? Flash { get; set; }
    
    public float? DelayTimeMilliseconds { get; set; }
    
    public string? FocusDistance { get; set; }
    
    public DateTime? ShootingDate { get; set; }
    
    public Guid UserId { get; set; }

    public string FileName { get; set; } = string.Empty;
    
    public long FileSizeBytes { get; set; }

    public string GetExtension() => Path.GetExtension(FileName) ?? string.Empty;
}