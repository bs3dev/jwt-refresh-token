# Jwt Refresh Token
.NET library to allowing a client application get new access tokens

### Introduction

Jwt Refresh Token is .Net library to provide a importante authentication aspects, and using Jwt Token (learn more [jwt.io](https://jwt.io) web site) 
and **Refresh Token**.

**Refresh Token** basically require a unique token identifier to obtain additional access tokens. Access token arent'n valid for an long period for security 
and **Refresh Token** strategy can help to re-authentication a user without login credential 🤔 (some scratches risks here)

### Architecture
This project is based in Onion Architecture created by [Jeffrey Palermo](https://jeffreypalermo.com/2008/07/the-onion-architecture-part-1/) in 2008.

* [Jwt.Refresh.Token.Infrastructure]()
* [Jwt.Refresh.Token.Application]()
* [Jwt.Refresh.Token.Domain]()
* [Jwt.Refresh.Token.Tests]()

#### Flow
![Miro](https://i.imgur.com/f8y4CGR.jpg)

#### ✅ Supported Databases

The goal of this project is to support the most widely used Azure-compatible databases:

- [x] [Azure Cosmos DB for NoSQL](https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/quickstart-dotnet) *(preview)*
- [ ] [Azure Cosmos DB for PostgreSQL](https://learn.microsoft.com/en-us/azure/cosmos-db/postgresql/introduction) *(planned)*
- [ ] [Azure SQL Database](https://learn.microsoft.com/en-us/azure/azure-sql/database/sql-database-paas-overview) *(planned)*
- [ ] [Azure Cosmos DB for MongoDB](https://learn.microsoft.com/en-us/azure/cosmos-db/mongodb/introduction) *(planned)*

#### Cosmos DB
To install Jwt.Refresh.Token.Cosmos *(include prereleases)*, run the following command in the [.NET CLI](https://learn.microsoft.com/en-us/dotnet/core/tools/)
```
dotnet add package Jwt.Refresh.Token.Cosmos --prerelease
```
##### Usage

1. ✯ Cosmos setup

The first step to do is provision your cosmos db, or if you already have it, create the token container id (*choose name you want*) with partitionKey **'/userId'**.

Implement IUserRepository for get user by id and password. UserId

2. ✯ Configure settings app:
```json
{
  "JwtRefreshToken": {
    "Descriptor": {
      "Issuer": "https://your-resource.com",
      "Audience": "https://your-audience.com",
      "AlgorithmKey": "YOUR_ALGORITHM_KEYr"
    },
    "Expires": {
      "CreateExpiresInMs": 60000,
      "RefreshExpiresInMs": 1209600000
    },
    "Cosmos": {
      "ConnectionString": "YOUR_COSMOS_CONNECTIONSTRING",
      "DatabaseName": "YOUR_DATABASEID",
      "TokenContainerId": "YOUR_TOKEN_CONTAINERID"
    }
  }
}
```

3. ✯ Configure startup app:

Install ASP.NET Core authentication middleware
```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```
and configuring it in your application’s startup class file: 

```csharp
// [required (cosmos)]  Add jwt cosmos repositories
builder.Services.AddJwtRefreshTokenCosmosServices(builder.Configuration);

// [required] AspNetCore Authentication config
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    // choose your bearer config 
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = true;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
              .GetBytes(builder.Configuration.GetValue<string>("JwtRefreshTokenDescriptor:AlgorithmKey"))),
            ValidateIssuer = true,
            ValidateAudience = true
        };
    });

// [required] AspNetCore Authentication config
builder.Services
    .AddAuthorization(auth =>
    {
        auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser().Build());
    });
```

4. ✯ Create token controller

Creating token controler to management token:

```csharp
[ApiController]
[Route("[controller]")]
public class TokenController : ControllerBase
{
    private readonly ILogger<TokenController> _logger;
    private readonly ITokenAppService _tokenAppService;
    private readonly IOptionsSnapshot<JwtRefreshTokenExpiresOptions> _jwtRefreshTokenExpiresOptions;

    public TokenController(ILogger<TokenController> logger, 
        ITokenAppService tokenAppService,
        IOptionsSnapshot<JwtRefreshTokenExpiresOptions> jwtRefreshTokenExpiresOptions)
    {
        _logger = logger;
        _tokenAppService = tokenAppService;
        _jwtRefreshTokenExpiresOptions = jwtRefreshTokenExpiresOptions;
    }

    private string GetRemoteIpAddress()
    {
        return this.Request?.HttpContext?.Connection?
            .RemoteIpAddress?.ToString();
    }

    [HttpPost("")]
    public async Task<IActionResult> PostAsync([FromForm] string userId, 
      [FromForm] string password, CancellationToken cancellationToken)
    {
        var token = await _tokenAppService.CreateAsync(userId, 
            password, _jwtRefreshTokenExpiresOptions.Value.CreateMilliseconds,
            GetRemoteIpAddress(), cancellationToken);

        return new TokenResult(token);
    }

    [Authorize("Bearer")]
    [HttpPatch("")]
    public async Task<IActionResult> RefreshAsync([FromForm] string tokenId,
      [FromForm] string userId, CancellationToken cancellationToken)
    {
        var token = await _tokenAppService.RefreshAsync(tokenId, 
            userId, _jwtRefreshTokenExpiresOptions.Value.RefreshMilliseconds,
            GetRemoteIpAddress(), cancellationToken);

        return new TokenResult(token);
    }

    [Authorize("Bearer")]
    [HttpPatch("/revoke")]
    public async Task<IActionResult> RevokeAsync([FromForm] string tokenId,
      [FromForm] string userId, CancellationToken cancellationToken)
    {
        var updated = await _tokenAppService.TryRevokeAsync(tokenId, 
            userId, GetRemoteIpAddress(), cancellationToken);

        return Ok(new { updated = updated });
    }
}
```

See integration test api [here](https://github.com/brunobrandes/jwt-refresh-token/tree/main/src/Tests/Jwt.Refresh.Token.Tests.Integrations.Api)

### TODO

- [ ] Incrise unit test coverage
- [ ] Create pipeline
- [ ] Implement PostgreeSql infrastructure
- [ ] Implement Sql Databse infrastructure
