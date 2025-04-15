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
    /// Registers the Cosmos DB implementation for ITokenRepository, including validated configuration.
    /// </summary>
    /// <param name="services">The service collection to extend.</param>
    /// <param name="configuration">The application configuration source.</param>
    /// <param name="cosmosClientOptions">Options to configure the Cosmos DB client.</param>
    public static void AddJwtRefreshTokenCosmosServices(
        this IServiceCollection services,
        IConfiguration configuration,
        CosmosClientOptions cosmosClientOptions)
    {
        // Register core JWT refresh token services (Descriptor, Expires, ITokenAppService, etc.)
        services.AddJwtRefreshTokenServices(configuration);

        // Bind and validate Cosmos-specific configuration
        services
            .AddOptions<JwtRefreshTokenCosmosConfig>()
            .Bind(configuration.GetSection(JwtRefreshTokenCosmosConfig.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Register the config as its shared interface
        services.AddSingleton<IJwtRefreshTokenDatabaseConfig>(sp =>
            sp.GetRequiredService<IOptions<JwtRefreshTokenCosmosConfig>>().Value);

        // Register the token repository using Cosmos DB
        services.AddScoped<ITokenRepository>(sp =>
        {
            var config = sp.GetRequiredService<IOptions<JwtRefreshTokenCosmosConfig>>().Value;
            return new TokenRepository(new CosmosClient(config.ConnectionString, cosmosClientOptions),
                config.DatabaseName, config.TokenContainerId);
        });

        // Force validation to run even if nothing directly resolves the config yet
        services.PostConfigure<JwtRefreshTokenCosmosConfig>(_ => { });
    }
}
