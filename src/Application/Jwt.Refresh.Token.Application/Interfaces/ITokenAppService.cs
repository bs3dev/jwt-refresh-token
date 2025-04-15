namespace Jwt.Refresh.Token.Application.Interfaces;

/// <summary>
/// Application service responsible for managing JWT token operations,
/// including creation, refresh, and revocation.
/// </summary>
public interface ITokenAppService
{
    /// <summary>
    /// Authenticates the user and generates a new access token.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="password">The user's password.</param>
    /// <param name="ipAddress">The IP address of the client making the request.</param>
    /// <param name="expiresMilliseconds">Token expiration time in milliseconds.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated token object.</returns>
    Task<Domain.DataTransferObjects.Token> CreateAsync(
        string userId,
        string password,
        int expiresMilliseconds,
        string ipAddress,
        CancellationToken cancellationToken);

    /// <summary>
    /// Refreshes an existing token by its ID and user ID.
    /// </summary>
    /// <param name="tokenId">The ID of the token to refresh.</param>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="ipAddress">The IP address of the client making the request.</param>
    /// <param name="expiresMilliseconds">New expiration time in milliseconds.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The refreshed token object.</returns>
    Task<Domain.DataTransferObjects.Token> RefreshAsync(
        string tokenId,
        string userId,
        int expiresMilliseconds,
        string ipAddress,
        CancellationToken cancellationToken);

    /// <summary>
    /// Attempts to revoke a token by its ID and user ID.
    /// </summary>
    /// <param name="tokenId">The ID of the token to revoke.</param>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="ipAddress">The IP address of the client making the request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of tokens revoked (0 or 1).</returns>
    Task<int> TryRevokeAsync(
        string tokenId,
        string userId,
        string ipAddress,
        CancellationToken cancellationToken);
}