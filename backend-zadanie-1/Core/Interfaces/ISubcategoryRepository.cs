using Core.Entities;

namespace Core.Interfaces;

public interface ISubcategoryRepository
{
    Task<IEnumerable<Subcategory>> GetAllAsync();
    Task<IEnumerable<Subcategory>> GetByCategoryIdAsync(Guid categoryId);
}
