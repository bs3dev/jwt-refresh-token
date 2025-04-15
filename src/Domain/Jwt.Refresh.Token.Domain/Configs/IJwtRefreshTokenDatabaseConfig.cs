namespace Jwt.Refresh.Token.Domain.Configs;

public interface IJwtRefreshTokenDatabaseConfig
{
    string ConnectionString { get; set; }
    string DatabaseName { get; set; }
}