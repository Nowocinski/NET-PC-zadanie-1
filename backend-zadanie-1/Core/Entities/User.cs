namespace Core.Entities;

public class User : BaseEntity
{
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation property
    public ICollection<Contact> Contacts { get; set; } = null!;
}