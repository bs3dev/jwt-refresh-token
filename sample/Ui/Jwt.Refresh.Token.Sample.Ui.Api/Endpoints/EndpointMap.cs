namespace Jwt.Refresh.Token.Sample.Ui.Api.Endpoints;

/// <summary>
/// Extension methods to map JWT refresh token endpoints in a minimal API.
/// </summary>
public static class EndpointMap
{
    /// <summary>
    /// Maps the endpoints for token creation, refresh, and revocation.
    /// </summary>
    public static IEndpointRouteBuilder MapTokenEndpoints(this IEndpointRouteBuilder app)
    {
        return app;
    }
}
