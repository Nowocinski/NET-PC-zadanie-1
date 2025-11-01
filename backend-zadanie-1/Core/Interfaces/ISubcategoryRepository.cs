using Core.Entities;

namespace Core.Interfaces;

public interface ISubcategoryRepository
{
    Task<IEnumerable<Subcategory>> GetByCategoryNameAsync(string name);
    Task<Subcategory?> AddAsync(string subcategoryName, string categoryName);
}
