using System.Text;
using Jwt.Refresh.Token.Domain.Entities.Repositories;
using Jwt.Refresh.Token.Infra.Cosmos.Extensions;
using Jwt.Refresh.Token.Sample.Ui.Api.Entities.Repositories;
using Microsoft.Azure.Cosmos;
using Microsoft.IdentityModel.Tokens;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAntiforgery(options =>
{
    // Allow client-side tools (like Swagger or Postman) to access the antiforgery cookie.
    // This is necessary to manually include the cookie in requests for CSRF validation.
    options.Cookie.HttpOnly = false;

    // Set the header name that the antiforgery system expects.
    // This should match what the client sends (e.g., X-XSRF-TOKEN).
    options.HeaderName = "X-XSRF-TOKEN";
});

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

// Registers a custom implementation of IUserRepository used to validate credentials and retrieve the userId,
// which is later assigned to the ClaimTypes.NameIdentifier in the generated JWT.
builder.Services.AddScoped<IUserRepository, UserRepository>();

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

// Map refresh token endpoints

app.Run();