namespace Jwt.Refresh.Token.Domain.Entities.Repositories;

public interface ITokenRepository
{
    Task<TokenEntity> CreateAsync(TokenEntity tokenEntity, CancellationToken cancellationToken);

    Task<TokenEntity> GetAsync(string tokenId, string userId, CancellationToken cancellationToken );

    Task<int> UpdateAsync(TokenEntity tokenEntity, CancellationToken cancellationToken );
}