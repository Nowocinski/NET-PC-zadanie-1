using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SubcategoryRepository(ApplicationDbContext context) : ISubcategoryRepository
{
    public async Task<IEnumerable<Subcategory>> GetAllAsync() =>
        await context.Subcategories
            .ToListAsync();

    public async Task<IEnumerable<Subcategory>> GetByCategoryNameAsync(string name) =>
        await context.Subcategories
            .Where(s => s.Category != null && s.Category.Name == name)
            .ToListAsync();

    public async Task<Subcategory?> AddAsync(string subcategoryName, string categoryName)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(c => c.Name == categoryName);

        if (category == null)
            return null;

        var existingSubcategory = await context.Subcategories.SingleOrDefaultAsync(s => s.Name == subcategoryName && s.CategoryId == category.Id);
        if (existingSubcategory != null)
            return existingSubcategory;

        var subcategory = new Subcategory
        {
            Id = Guid.NewGuid(),
            Name = subcategoryName,
            CategoryId = category.Id
        };

        await context.Subcategories.AddAsync(subcategory);
        await context.SaveChangesAsync();

        return subcategory;
    }
}
