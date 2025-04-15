namespace Jwt.Refresh.Token.Domain.Entities;

public class TokenEntity
{
    public string Id { get; set; }

    public string UserId { get; set; }

    public string AccessToken { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public string CreatedIpAddress { get; set; }

    public DateTimeOffset? RevokedAt { get; set; }

    public string RevokedIpAddress { get; set; }

    public int Ttl { get; set; } = -1;
}