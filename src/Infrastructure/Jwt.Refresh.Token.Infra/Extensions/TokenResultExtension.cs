using System.Text.Json;
using Jwt.Refresh.Token.Domain.Constants;
using Microsoft.AspNetCore.Http;

namespace Jwt.Refresh.Token.Infra.Extensions;

public static class TokenResultExtension
{
    public static async Task ExecuteAsync(this HttpContext httpContext, Domain.DataTransferObjects.Token token)
    {
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = token.Status;

        if (token.Status == TokenStatusConst.Authorized)
        {
            await httpContext.Response.Body.WriteAsync(JsonSerializer.SerializeToUtf8Bytes(token));
        }
    }
}