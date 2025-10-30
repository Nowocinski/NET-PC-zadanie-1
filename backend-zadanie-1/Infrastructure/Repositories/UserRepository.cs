using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context, JwtTokenService tokenService) : IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email) =>
        await context.Users
            .Include(u => u.Contacts)
            .FirstOrDefaultAsync(u => u.Email == email);

    public async Task<(string accessToken, string refreshToken)?> GenerateTokensAsync(string email, string password)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        
        if (user == null)
            return null;

        // Verify password using BCrypt
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return null;

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        return (accessToken, refreshToken);
    }
}
