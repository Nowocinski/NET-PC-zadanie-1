using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SubcategoryRepository(ApplicationDbContext context) : ISubcategoryRepository
{
    public async Task<IEnumerable<Subcategory>> GetAllAsync() =>
        await context.Subcategories
            .Include(s => s.Category)
            .ToListAsync();

    public async Task<IEnumerable<Subcategory>> GetByCategoryIdAsync(Guid categoryId) =>
        await context.Subcategories
            .Include(s => s.Category)
            .Where(s => s.CategoryId == categoryId)
            .ToListAsync();
}
