using System.Net;
using Jwt.Refresh.Token.Domain.Entities;
using Jwt.Refresh.Token.Domain.Entities.Repositories;
using Microsoft.Azure.Cosmos;

namespace Jwt.Refresh.Token.Infra.Cosmos.Entities.Repositories;

/// <summary>
    /// Repository responsible for persisting and retrieving token entities using Azure Cosmos DB.
    /// </summary>
    public class TokenRepository : ITokenRepository
    {
        private readonly Container _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenRepository"/> class.
        /// </summary>
        /// <param name="cosmosClient">Cosmos DB client instance.</param>
        /// <param name="databaseName">The name of the Cosmos DB database.</param>
        /// <param name="containerId">The ID of the container where tokens are stored.</param>
        public TokenRepository(CosmosClient cosmosClient, string databaseName, string containerId)
        {
            _container = cosmosClient.GetContainer(databaseName, containerId);
        }

        /// <summary>
        /// Creates a new token entity in Cosmos DB.
        /// </summary>
        /// <param name="tokenEntity">The token entity to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created token entity.</returns>
        public async Task<TokenEntity> CreateAsync(TokenEntity tokenEntity, CancellationToken cancellationToken)
        {
            var response = await _container.CreateItemAsync(tokenEntity, new PartitionKey(tokenEntity.UserId), cancellationToken: cancellationToken);
            return response.Resource;
        }

        /// <summary>
        /// Retrieves a token entity by ID and user ID (used as partition key).
        /// </summary>
        /// <param name="tokenId">The token ID.</param>
        /// <param name="userId">The user ID (partition key).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The token entity if found; otherwise, null.</returns>
        public async Task<TokenEntity> GetAsync(string tokenId, string userId, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _container.ReadItemAsync<TokenEntity>(tokenId, new PartitionKey(userId), cancellationToken: cancellationToken);
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return default;
            }
        }

        /// <summary>
        /// Updates an existing token entity in Cosmos DB.
        /// </summary>
        /// <param name="tokenEntity">The token entity to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The number of documents affected (0 or 1).</returns>
        public async Task<int> UpdateAsync(TokenEntity tokenEntity, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _container.ReplaceItemAsync(
                    item: tokenEntity,
                    id: tokenEntity.Id,
                    partitionKey: new PartitionKey(tokenEntity.UserId),
                    cancellationToken: cancellationToken
                );

                return response.StatusCode == HttpStatusCode.OK ? 1 : 0;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return 0;
            }
        }
    }