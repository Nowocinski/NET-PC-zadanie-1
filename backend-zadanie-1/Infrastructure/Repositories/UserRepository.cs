using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email) =>
        await context.Users
            .Include(u => u.Contacts)
            .FirstOrDefaultAsync(u => u.Email == email);
}
