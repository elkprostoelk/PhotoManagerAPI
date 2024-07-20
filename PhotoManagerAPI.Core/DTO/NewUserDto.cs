namespace PhotoManagerAPI.Core.DTO;

public class NewUserDto
{
    public string UserName { get; set; } = string.Empty;
    
    public string? FullName { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = "User";

    public string Password { get; set; } = string.Empty;
    
    public string ConfirmPassword { get; set; } = string.Empty;
}