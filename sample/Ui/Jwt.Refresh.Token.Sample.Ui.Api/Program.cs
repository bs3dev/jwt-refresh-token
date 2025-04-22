using System.Text;
using System.Text.Json;
using Jwt.Refresh.Token.Domain.Entities.Repositories;
using Jwt.Refresh.Token.Infra.AspNetCore.Endpoints;
using Jwt.Refresh.Token.Infra.AspNetCore.Serializers;
using Jwt.Refresh.Token.Infra.Cosmos.Extensions;
using Jwt.Refresh.Token.Sample.Ui.Api.Entities.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.IdentityModel.Tokens;
var builder = WebApplication.CreateBuilder(args);

// 1) CSRF protection
builder.Services.AddAntiforgery(options =>
{
    // Allow client-side tools (like Swagger or Postman) to access the antiforgery cookie.
    // This is necessary to manually include the cookie in requests for CSRF validation.
    options.Cookie.HttpOnly = false;

    // Set the header name that the antiforgery system expects.
    // This should match what the client sends (e.g., X-XSRF-TOKEN).
    options.HeaderName = "X-XSRF-TOKEN";
});

// 2) Minimal API & JSON options
// Registers the source-generated System.Text.Json context used for serializing tokens and responses.
// This improves performance and avoids runtime errors when reflection-based serialization is disabled.
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.TypeInfoResolverChain.Add(AppJsonContext.Default);
});

// 3) Register core services
// Register refresh token services
builder.Services.AddJwtRefreshTokenCosmosServices(builder.Configuration, new CosmosClientOptions
{
    ConnectionMode = ConnectionMode.Gateway,
    AllowBulkExecution = true,
    SerializerOptions = new CosmosSerializationOptions
    {
        Indented = true,
        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
        IgnoreNullValues = true
    }
});

// 4) Register custom user repository
// Registers a custom implementation of IUserRepository used to validate credentials and retrieve the userId,
// which is later assigned to the ClaimTypes.NameIdentifier in the generated JWT.
builder.Services.AddScoped<IUserRepository, UserRepository>();

// 5) Authentication & Authorization
// 🔐 Secure JWT Authentication Configuration
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(builder.Configuration["JwtRefreshToken:Descriptor:AlgorithmKey"]
                                    ?? throw new InvalidOperationException("JWT key not configured."))
        ),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtRefreshToken:Descriptor:Issuer"],
        
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtRefreshToken:Descriptor:Audience"],

        ValidateLifetime = true,
        RequireExpirationTime = true,
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

// Configures the default authorization policy used by the protected endpoints.
// The "Bearer" policy requires a valid JWT token and is associated with the "Bearer" authentication scheme.
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Bearer", policy =>
    {
        policy.AddAuthenticationSchemes("Bearer");
        policy.RequireAuthenticatedUser();
    });
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Use CSRF protection middleware
app.UseAntiforgery();

// 6) Map built‑in refresh token endpoints
// Maps the default refresh token endpoints (POST /token, PATCH /token, PATCH /revoke).
// These endpoints are fully integrated with antiforgery and JWT security mechanisms.
// If needed, you can implement your own custom routes by invoking ITokenAppService directly.
app.MapTokenEndpoints("token");

app.Run();