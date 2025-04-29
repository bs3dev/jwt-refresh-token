using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Jwt.Refresh.Token.Domain.Configs;
using Jwt.Refresh.Token.Domain.Entities.Repositories;
using Jwt.Refresh.Token.Extensions;
using Jwt.Refresh.Token.Infra.Cosmos.Configs;
using Jwt.Refresh.Token.Infra.Cosmos.Entities.Repositories;

namespace Jwt.Refresh.Token.Infra.Cosmos.Extensions;

/// <summary>
/// Extension methods for registering Cosmos DB-based JWT refresh token infrastructure.
/// </summary>
public static class DiExtension
{
    /// <summary>
    /// Registers the Cosmos DB implementation for ITokenRepository, expecting a CosmosClient to be provided.
    /// </summary>
    /// <param name="services">The service collection to extend.</param>
    /// <param name="configuration">The application configuration source.</param>
    public static void AddJwtRefreshTokenCosmosServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register core JWT refresh token services (Descriptor, Expires, ITokenAppService, etc.)
        services.AddJwtRefreshTokenServices(configuration);

        // Bind and validate Cosmos-specific configuration
        services
            .AddOptions<JwtRefreshTokenCosmosConfig>()
            .Bind(configuration.GetSection(JwtRefreshTokenCosmosConfig.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IJwtRefreshTokenDatabaseConfig>(sp =>
            sp.GetRequiredService<IOptions<JwtRefreshTokenCosmosConfig>>().Value);

        // Expect that a CosmosClient is already registered
        services.AddScoped<ITokenRepository>(sp =>
        {
            var config = sp.GetRequiredService<IOptions<JwtRefreshTokenCosmosConfig>>().Value;
            var cosmosClient = sp.GetRequiredService<CosmosClient>();
            return new TokenRepository(cosmosClient, config.DatabaseName, config.TokenContainerId);
        });

        services.PostConfigure<JwtRefreshTokenCosmosConfig>(_ => { });
    }
}
