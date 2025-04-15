using Jwt.Refresh.Token.Application.Interfaces;
using Jwt.Refresh.Token.Domain.Entities.Repositories;
using Jwt.Refresh.Token.Infra.Cosmos.Configs;
using Jwt.Refresh.Token.Infra.Cosmos.Extensions;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jwt.Refresh.Token.Tests.Integration.Cosmos.Fixtures;

/// <summary>
/// Shared fixture that sets up dependency injection and Cosmos DB for integration testing.
/// </summary>
public class TokenTestFixture : IAsyncLifetime
{
    /// <summary>
    /// Service provider for resolving application services during tests.
    /// </summary>
    public IServiceProvider Services { get; private set; } = null!;

    /// <summary>
    /// Exposes the main application service for managing tokens.
    /// </summary>
    public ITokenAppService TokenAppService => Services.GetRequiredService<ITokenAppService>();

    private IConfiguration _configuration = null!;
    private CosmosClientOptions _cosmosClientOptions = null!;

    /// <summary>
    /// Initializes the Cosmos DB client and service container for integration testing.
    /// </summary>
    public async Task InitializeAsync()
    {
        var services = new ServiceCollection();

        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        _cosmosClientOptions = new CosmosClientOptions
        {
            ConnectionMode = ConnectionMode.Gateway,
            AllowBulkExecution = true,
            SerializerOptions = new CosmosSerializationOptions
            {
                Indented = true,
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                IgnoreNullValues = true
            }
        };

        services.AddJwtRefreshTokenCosmosServices(_configuration, _cosmosClientOptions);
        
        services.AddSingleton<IUserRepository, FakeUserRepository>();

        Services = services.BuildServiceProvider();

        await CreateDbConfigIfNotExistsAsync();
    }

    /// <summary>
    /// Ensures the configured Cosmos DB database and container exist.
    /// </summary>
    private async Task CreateDbConfigIfNotExistsAsync()
    {
        var config = _configuration
            .GetSection(JwtRefreshTokenCosmosConfig.SectionName)
            .Get<JwtRefreshTokenCosmosConfig>()!;

        var client = new CosmosClient(config.ConnectionString, _cosmosClientOptions);

        var database = await client.CreateDatabaseIfNotExistsAsync(config.DatabaseName);

        var containerProperties = new ContainerProperties(
            id: config.TokenContainerId,
            partitionKeyPath: "/userId"
        )
        {
            DefaultTimeToLive = -1
        };

        await database.Database.CreateContainerIfNotExistsAsync(containerProperties);
    }

    /// <summary>
    /// Performs cleanup after tests are executed.
    /// </summary>
    public Task DisposeAsync() => Task.CompletedTask;
}
