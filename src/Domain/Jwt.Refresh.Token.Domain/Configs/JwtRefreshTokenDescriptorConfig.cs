using System.ComponentModel.DataAnnotations;

namespace Jwt.Refresh.Token.Domain.Configs;

public class JwtRefreshTokenDescriptorConfig
{
    [Required]
    public string AlgorithmKey { get; set; }        
    [Required]
    public string Issuer { get; set; }
    [Required]
    public string Audience { get; set; }
}