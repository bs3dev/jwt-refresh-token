using Jwt.Refresh.Token.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Jwt.Refresh.Token.Application.Interfaces;
using Jwt.Refresh.Token.Domain.Configs;
using Jwt.Refresh.Token.Domain.Services;
using Jwt.Refresh.Token.Domain.Services.Interfaces;

namespace Jwt.Refresh.Token.Extensions;

/// <summary>
/// Extension methods for setting up JWT-related services and configurations.
/// </summary>
public static class DiExtension
{
    /// <summary>
    /// Registers JWT token services and binds core configuration with validation.
    /// </summary>
    /// <param name="services">The service collection to which services will be added.</param>
    /// <param name="configuration">The application's configuration source.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddJwtRefreshTokenServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind and validate JwtRefreshTokenConfig (Descriptor + Expires)
        services
            .AddOptions<JwtRefreshTokenConfig>()
            .Bind(configuration.GetSection(JwtRefreshTokenConfig.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Register core services
        services.AddScoped<IClaimsIdentityService, ClaimsIdentityService>();
        services.AddScoped<IJwtAccessTokenService, JwtAccessTokenService>();
        services.AddScoped<ITokenAppService, TokenAppService>();

        return services;
    }
}