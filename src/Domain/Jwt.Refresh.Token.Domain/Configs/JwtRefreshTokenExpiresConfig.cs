namespace Jwt.Refresh.Token.Domain.Configs;

public class JwtRefreshTokenExpiresConfig
{
    public int CreateExpiresInMs { get; set; }
    public int RefreshExpiresInMs { get; set; }
}