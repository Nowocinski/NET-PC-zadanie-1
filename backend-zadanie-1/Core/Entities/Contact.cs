namespace Core.Entities;

public class Contact : BaseEntity
{
    public Guid UserId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Phone { get; set; }
    public required DateTime BirthDate { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? SubcategoryId { get; set; }
    
    // Navigation properties
    public User? User { get; set; }
    public Category? Category { get; set; }
    public Subcategory? Subcategory { get; set; }
}