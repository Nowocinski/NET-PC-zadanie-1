using Core.Entities;

namespace Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task<(string accessToken, string refreshToken)?> GenerateTokensAsync(string email, string password);
    Task<(string accessToken, string refreshToken)?> RefreshTokensAsync(string accessToken, string refreshToken);
    Task<User?> RegisterAsync(string email, string password, string name);
}
