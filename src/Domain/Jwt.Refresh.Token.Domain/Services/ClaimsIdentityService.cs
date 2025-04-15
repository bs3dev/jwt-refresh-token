using System.Security.Claims;
using Jwt.Refresh.Token.Domain.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Jwt.Refresh.Token.Domain.Services;

public class ClaimsIdentityService : IClaimsIdentityService
{
    private readonly IServiceProvider _serviceProvicer;

    public ClaimsIdentityService(IServiceProvider serviceProvicer)
    {
        _serviceProvicer = serviceProvicer;
    }

    public async Task<ClaimsIdentity> GetAsync(string userId, CancellationToken cancellationToken = default)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };

        var claimsService = _serviceProvicer.GetService<IClaimsService>();

        if (claimsService is not null)
        {
            var customClaims =  await claimsService.GetAsync(userId, cancellationToken);

            if (customClaims is not null && customClaims.Count() > 0)
            {
                claims.AddRange(customClaims.Where(x => x.Type != ClaimTypes.NameIdentifier));
            }
        }

        return new ClaimsIdentity(claims);
    }
}