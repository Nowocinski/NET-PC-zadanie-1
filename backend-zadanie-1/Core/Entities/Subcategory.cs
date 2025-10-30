namespace Core.Entities;

public class Subcategory : BaseEntity
{
    public Guid CategoryId { get; set; }
    public required string Name { get; set; }
}