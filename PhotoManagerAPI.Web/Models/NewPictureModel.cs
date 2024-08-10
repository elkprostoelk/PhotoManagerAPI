namespace PhotoManagerAPI.Web.Models;

public class NewPictureModel
{
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }

    public required IFormFile File { get; set; }
    
    public int Width { get; set; }
    
    public int Height { get; set; }
    
    public string? Iso { get; set; }
    
    public string? CameraModel { get; set; }
    
    public bool? Flash { get; set; }
    
    public float? DelayTimeMilliseconds { get; set; }
    
    public string? FocusDistance { get; set; }
    
    public DateTime? ShootingDate { get; set; }
}