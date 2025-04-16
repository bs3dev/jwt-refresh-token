using System.Text;
using Jwt.Refresh.Token.Sample.Ui.Api.Endpoints;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 🔐 Secure JWT Authentication Configuration
builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
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

builder.Services.AddAuthorization();

// Register refresh token services

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Map refresh token endpoints
app.MapTokenEndpoints();

app.Run();