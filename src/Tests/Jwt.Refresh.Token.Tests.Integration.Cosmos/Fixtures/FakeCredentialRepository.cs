using Jwt.Refresh.Token.Domain.Entities.Repositories;

namespace Jwt.Refresh.Token.Tests.Integration.Cosmos.Fixtures;

/// <summary>
/// Fake user repository for integration testing. Always returns the userId if input is valid.
/// </summary>
public class FakeCredentialRepository : ICredentialRepository
{
    /// <summary>
    /// Returns the userId if both userId and password are not empty; otherwise, returns null.
    /// </summary>
    public Task<string> GetAsync(string userId, string password, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(userId) && !string.IsNullOrWhiteSpace(password))
            return Task.FromResult(userId);

        return Task.FromResult<string>(null);
    }
}