namespace PhotoManagerAPI.DataAccess.Entities;

public class User
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    
    public string? FullName { get; set; }
    
    public int RoleId { get; set; }
    
    public Role? Role { get; set; }
    
    public string Salt { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreationDate { get; set; }
}