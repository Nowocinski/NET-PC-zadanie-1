namespace Core.Entities;

public class Category : BaseEntity
{
    public required string Name { get; set; }
    
    // Navigation properties
    public ICollection<Subcategory> Subcategories { get; set; } = null!;
    public ICollection<Contact> Contacts { get; set; } = null!;
}