using System.Security.Claims;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace Jwt.Refresh.Token.Infra.Extensions;

/// <summary>
/// Extension methods for issuing antiforgery tokens tied to a specific user identity.
/// </summary>
public static class AntiforgeryExtension
{
    private const string XsrfHeaderName = "X-XSRF-TOKEN";
    /// <summary>
    /// Creates a minimal ClaimsPrincipal containing only the NameIdentifier claim for the specified userId,
    /// assigns it to HttpContext.User, and issues an antiforgery cookie plus the X-XSRF-TOKEN header.
    /// </summary>
    /// <param name="httpContext">The current HTTP context.</param>
    /// <param name="antiforgery">The antiforgery service.</param>
    /// <param name="userId">The user identifier to bind to the antiforgery token.</param>
    public static void IssueAntiforgeryToken(this HttpContext httpContext, IAntiforgery antiforgery, string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));    
        
        // 1) Build a minimal identity with NameIdentifier claim
        var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "JwtRefresh");
        
        httpContext.User = new ClaimsPrincipal(identity);

        // 2) Generate and persist antiforgery cookie and header
        var tokens = antiforgery.GetAndStoreTokens(httpContext);
        httpContext.Response.Headers[XsrfHeaderName] = tokens.RequestToken!;
    }
}