using Core.Entities;

namespace Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<(string accessToken, string refreshToken)?> GenerateTokensAsync(string email, string password);
}
