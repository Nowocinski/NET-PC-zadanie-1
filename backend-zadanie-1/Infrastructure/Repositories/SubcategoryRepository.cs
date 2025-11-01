using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SubcategoryRepository(ApplicationDbContext context) : ISubcategoryRepository
{
    public async Task<IEnumerable<Subcategory>> GetByCategoryNameAsync(string name) =>
        await context.Subcategories
            .Include(s => s.Category)
            .Where(s => s.Category != null && s.Category.Name == name)
            .ToListAsync();
}
