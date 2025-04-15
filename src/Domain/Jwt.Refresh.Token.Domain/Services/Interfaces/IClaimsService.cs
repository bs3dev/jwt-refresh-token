using System.Security.Claims;

namespace Jwt.Refresh.Token.Domain.Services.Interfaces;

public interface IClaimsService
{
    Task<IEnumerable<Claim>> GetAsync(string userId, CancellationToken cancellationToken);
}