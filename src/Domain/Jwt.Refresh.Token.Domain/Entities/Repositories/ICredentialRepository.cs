namespace Jwt.Refresh.Token.Domain.Entities.Repositories;

public interface ICredentialRepository
{
    /// <summary>
    /// Queries the user ID based on the given credentials. Returns the user ID if found and valid; otherwise, returns null.
    /// </summary>
    /// <param name="userId">User identifier</param>
    /// <param name="password">User password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>UserId if valid credentials</returns>
    Task<string?> GetAsync(string userId, string password, CancellationToken cancellationToken);
}