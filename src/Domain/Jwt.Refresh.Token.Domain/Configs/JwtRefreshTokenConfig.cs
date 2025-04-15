using System.ComponentModel.DataAnnotations;

namespace Jwt.Refresh.Token.Domain.Configs;

public class JwtRefreshTokenConfig
{
    public const string SectionName = "JwtRefreshToken";
    
    [Required]
    public JwtRefreshTokenDescriptorConfig Descriptor { get; set; }
    
    [Required]
    public JwtRefreshTokenExpiresConfig Expires { get; set; }
}