using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Jwt.Refresh.Token.Domain.Configs;
using Jwt.Refresh.Token.Domain.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Jwt.Refresh.Token.Domain.Services;

/// <summary>
/// Service responsible for generating JWT access tokens.
/// </summary>
public sealed class JwtAccessTokenService : IJwtAccessTokenService
{
    private readonly IClaimsIdentityService _claimsIdentityService;
    private readonly JwtRefreshTokenConfig _jwtRefreshTokenConfig;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtTokenService"/> class.
    /// </summary>
    /// <param name="claimsIdentityService">Service that provides claims for the user.</param>
    /// <param name="optionsSnapshot">JWT configuration options.</param>
    public JwtAccessTokenService(
        IClaimsIdentityService claimsIdentityService,
        IOptionsSnapshot<JwtRefreshTokenConfig> optionsSnapshot)
    {
        _claimsIdentityService = claimsIdentityService;
        _jwtRefreshTokenConfig = optionsSnapshot.Value;
    }

    /// <inheritdoc />
    public async Task<string> GetAsync(string userId, int expiresMilliseconds, CancellationToken cancellationToken = default)
    {
        var tokenDescriptor = await CreateTokenDescriptorAsync(userId, expiresMilliseconds, cancellationToken);
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private async Task<SecurityTokenDescriptor> CreateTokenDescriptorAsync(string userId, int expiresMilliseconds, CancellationToken cancellationToken)
    {
        var identity = await _claimsIdentityService.GetAsync(userId, cancellationToken);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtRefreshTokenConfig.Descriptor.AlgorithmKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        return new SecurityTokenDescriptor
        {
            Subject = identity,
            Expires = DateTime.UtcNow.AddMilliseconds(expiresMilliseconds),
            SigningCredentials = credentials,
            Issuer = _jwtRefreshTokenConfig.Descriptor.Issuer,
            IssuedAt = !string.IsNullOrWhiteSpace(_jwtRefreshTokenConfig.Descriptor.Issuer) ? DateTime.UtcNow : null,
            Audience = _jwtRefreshTokenConfig.Descriptor.Audience
        };
    }
}
