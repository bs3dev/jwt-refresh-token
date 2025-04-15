using System.Security.Claims;

namespace Jwt.Refresh.Token.Domain.Services.Interfaces;

public interface IClaimsIdentityService
{
    Task<ClaimsIdentity> GetAsync(string userId, CancellationToken cancellationToken);
}