using Jwt.Refresh.Token.Application.Interfaces;
using Jwt.Refresh.Token.Domain.Configs;
using Jwt.Refresh.Token.Domain.Constants;
using Jwt.Refresh.Token.Infra.Extensions;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Jwt.Refresh.Token.Infra.AspNetCore.Endpoints;

// <summary>
/// Extension methods to map JWT refresh token endpoints in a minimal API.
/// </summary>
public static class TokenEndpointExtension
{
    /// <summary>
    /// Maps the endpoints for token creation, refresh, and revocation.
    /// </summary>
    public static IEndpointRouteBuilder MapTokenEndpoints(this IEndpointRouteBuilder app, string pattern)
    {
        app.MapPost($"/{pattern}", async (
            HttpContext context,
            [FromServices] ITokenAppService tokenAppService,
            [FromServices] IOptionsSnapshot<JwtRefreshTokenConfig> jwtRefreshTokenConfig,
            [FromServices] IAntiforgery antiforgery,
            [FromForm] string userId,
            [FromForm] string password,
            CancellationToken cancellationToken) =>
        {
            var token = await tokenAppService.CreateAsync(
                userId,
                password,
                jwtRefreshTokenConfig.Value.Expires.CreateExpiresInMs,
                context.Connection.RemoteIpAddress?.ToString(),
                cancellationToken);

            if (token.Status == TokenStatusConst.Authorized)
            {
                var tokens = antiforgery.GetAndStoreTokens(context);
                context.Response.Headers["X-XSRF-TOKEN"] = tokens.RequestToken!;  
            }
            
            await context.WriteAsync(token);
        })
            .DisableAntiforgery();

        app.MapPatch($"/{pattern}", async (
            HttpContext context,
            [FromServices] ITokenAppService tokenAppService,
            [FromServices] IOptionsSnapshot<JwtRefreshTokenConfig> jwtRefreshTokenConfig,
            [FromServices] IAntiforgery antiforgery,
            [FromForm] string tokenId,
            [FromForm] string userId,
            CancellationToken cancellationToken) =>
        {
            var token = await tokenAppService.RefreshAsync(
                tokenId,
                userId,
                jwtRefreshTokenConfig.Value.Expires.RefreshExpiresInMs,
                context.Connection.RemoteIpAddress?.ToString(),
                cancellationToken);
            
            if (token.Status == TokenStatusConst.Authorized)
            {
                var tokens = antiforgery.GetAndStoreTokens(context);
                context.Response.Headers["X-XSRF-TOKEN"] = tokens.RequestToken!;  
            }

            await context.WriteAsync(token);
        }).RequireAuthorization("Bearer");

        app.MapDelete($"/{pattern}",  async (
            HttpContext context,
            [FromServices] ITokenAppService tokenAppService,
            [FromForm] string tokenId,
            [FromForm] string userId,
            CancellationToken cancellationToken) =>
        {
            var updated = await tokenAppService.TryRevokeAsync(tokenId, userId, 
                context.Connection.RemoteIpAddress?.ToString(), cancellationToken);
            
            return Results.Ok(new { updated = updated });
        }).RequireAuthorization("Bearer");

        return app;
    }
}