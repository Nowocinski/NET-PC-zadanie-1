namespace Core.Entities;

public class Subcategory : BaseEntity
{
    public Guid CategoryId { get; set; }
    public required string Name { get; set; }
    
    // Navigation properties
    public Category? Category { get; set; }
    public ICollection<Contact> Contacts { get; set; } = null!;
}