using Jwt.Refresh.Token.Domain.Entities.Repositories;

namespace Jwt.Refresh.Token.Sample.Ui.Api.Entities.Repositories;

/// <summary>
/// Example implementation of ICredentialRepository for demonstration purposes only.
/// This simulates user credential validation using a static in-memory check.
/// </summary>
public class CredentialRepository : ICredentialRepository
{
    private static readonly UserEntity SampleUser = new UserEntity { };

    /// <summary>
    /// Validates user credentials and returns the user ID if valid.
    /// </summary>
    public Task<string?> GetAsync(string userId, string password, CancellationToken cancellationToken)
    {
        if (userId == SampleUser.UserId && password == SampleUser.Password)
            return Task.FromResult<string?>(SampleUser.UserId);

        return Task.FromResult<string?>(null);
    }
}