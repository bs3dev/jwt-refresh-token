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

```bash
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
      "Issuer": "YOUR_ISSUER",
      "Audience": "YOUR_AUDIENCE",
      "AlgorithmKey": "YOUR_ALGORITHM_KEY"
    },
    "Expires": {
     "CreateExpiresInMs": 300000,
  "RefreshExpiresInMs": 604800000
    },
    "Cosmos": {
      "ConnectionString": "YOUR_COSMOS_CONNECTIONSTRING",
      "DatabaseName": "YOUR_DATABASEID",
      "TokenContainerId": "YOUR_TOKEN_CONTAINERID"
    }
  }
}
```

3. ✯ Configure startup and endpoints app:

⚠️ **Note:** Although this library uses `System.Text.Json` internally, the `Microsoft.Azure.Cosmos` SDK still requires `Newtonsoft.Json` as a runtime dependency. Make sure to install it in your host application to avoid runtime errors:

```bash
dotnet add package Newtonsoft.Json 
```

Now, configure it in your application's Program.cs (startup file):

```csharp

builder.Services.AddAntiforgery(options =>
{
    // Allow client-side tools (like Swagger or Postman) to access the antiforgery cookie.
    // This is necessary to manually include the cookie in requests for CSRF validation.
    options.Cookie.HttpOnly = false;

    // Set the header name that the antiforgery system expects.
    // This should match what the client sends (e.g., X-XSRF-TOKEN).
    options.HeaderName = "X-XSRF-TOKEN";
});

// Registers the source-generated System.Text.Json context used for serializing tokens and responses.
// This improves performance and avoids runtime errors when reflection-based serialization is disabled.
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Add(AppJsonContext.Default);
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

// Maps the default refresh token endpoints (POST /token, PATCH /token, PATCH /revoke).
// These endpoints are fully integrated with antiforgery and JWT security mechanisms.
// If needed, you can implement your own custom routes by invoking ITokenAppService directly.
app.MapTokenEndpoints();

app.Run();

```

See integration test [here](https://github.com/brunobrandes/jwt-refresh-token/tree/main/src/Tests/Jwt.Refresh.Token.Tests.Integration.Cosmos)

---

### 🧪 Sample Project with Postman Support

A complete working example is available in the [`/sample`](./sample) folder.

It demonstrates:

- ✅ How to configure and run the minimal API
- 🔐 Full antiforgery protection (XSRF)
- 🧪 Token creation, refresh, and revocation endpoints
- 📬 A Postman Collection to test the full flow locally

📄 See full instructions in [`/sample/README.md`](./sample/README.md)

### TODO

- [x] Incrise unit test coverage
- [x] Create pipeline
- [ ] Implement Mongo DB infrastructure
- [ ] Implement PostgreeSql infrastructure
- [ ] Implement Sql Databse infrastructure
