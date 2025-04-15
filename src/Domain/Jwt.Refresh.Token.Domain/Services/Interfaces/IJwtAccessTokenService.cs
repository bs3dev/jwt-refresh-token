namespace Jwt.Refresh.Token.Domain.Services.Interfaces;

public interface IJwtAccessTokenService
{
    Task<string> GetAsync(string userId, int expiresSeconds, CancellationToken cancellationToken);
}