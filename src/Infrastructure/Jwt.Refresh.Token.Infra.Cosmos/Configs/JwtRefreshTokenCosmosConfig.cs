using System.ComponentModel.DataAnnotations;
using Jwt.Refresh.Token.Domain.Configs;

namespace Jwt.Refresh.Token.Infra.Cosmos.Configs;

public class JwtRefreshTokenCosmosConfig : IJwtRefreshTokenDatabaseConfig
{
    public const string SectionName = "JwtRefreshToken:Cosmos";
    
    [Required]
    public string ConnectionString { get; set; }
    
    [Required]
    public string DatabaseName { get; set; }
    
    [Required]
    public string TokenContainerId { get; set; }
}