using Jwt.Refresh.Token.Domain.Entities;

namespace Jwt.Refresh.Token.Domain.Extensions;

public static class TokenExtension
{
    public static TokenEntity ToRevokeTokenEntity(this TokenEntity tokenEntity, string ipAddress)
    {
        return new TokenEntity
        {
            Id = tokenEntity.Id,
            UserId = tokenEntity.UserId,
            AccessToken = tokenEntity.AccessToken,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedIpAddress = tokenEntity.CreatedIpAddress,
            RevokedAt = DateTimeOffset.UtcNow,
            RevokedIpAddress = ipAddress
        };
    }

    public static TokenEntity ToEntity(this DataTransferObjects.Token token, string ipAddress, int expiresMilliseconds)
    {
        return new TokenEntity
        {
            Id = token.Id,
            UserId = token.UserId,
            AccessToken = token.AccessToken,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedIpAddress = ipAddress,
            Ttl = (int)expiresMilliseconds.ToTimeSpanMilliseconds().TotalSeconds
        };  
    }
}