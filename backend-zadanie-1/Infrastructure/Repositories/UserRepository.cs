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

    public async Task<User?> GetByIdAsync(Guid id) =>
        await context.Users.FindAsync(id);

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

    public async Task<(string accessToken, string refreshToken)?> RefreshTokensAsync(string accessToken, string refreshToken)
    {
        // Validate the access token (without checking expiration)
        var principal = tokenService.ValidateToken(accessToken);
        if (principal == null)
            return null;

        // Get user ID from token claims
        var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return null;

        // Get user from database
        var user = await GetByIdAsync(userId);
        if (user == null)
            return null;

        // Generate new tokens
        var newAccessToken = tokenService.GenerateAccessToken(user);
        var newRefreshToken = tokenService.GenerateRefreshToken();

        return (newAccessToken, newRefreshToken);
    }

    public async Task<User?> RegisterAsync(string email, string password, string name)
    {
        var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (existingUser != null)
            return null;

        // Hash password using BCrypt
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = passwordHash,
            Name = name,
            CreatedAt = DateTime.UtcNow
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        return user;
    }
}
