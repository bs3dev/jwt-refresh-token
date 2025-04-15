using FluentAssertions;
using Jwt.Refresh.Token.Application.Interfaces;
using Jwt.Refresh.Token.Domain.Constants;
using Jwt.Refresh.Token.Tests.Integration.Cosmos.Fixtures;

namespace Jwt.Refresh.Token.Tests.Integration.Cosmos;

/// <summary>
/// Integration tests that validate the full token creation and refresh flow using Cosmos DB.
/// </summary>
public class TokenAppServiceIntegrationTests : IClassFixture<TokenTestFixture>
{
    private readonly ITokenAppService _tokenAppService;

    /// <summary>
    /// Initializes a new instance of the test class with the shared fixture.
    /// </summary>
    public TokenAppServiceIntegrationTests(TokenTestFixture fixture)
    {
        _tokenAppService = fixture.TokenAppService;
    }

    /// <summary>
    /// Validates that a token can be created and refreshed successfully using Cosmos DB.
    /// </summary>
    [Fact]
    public async Task Should_Create_And_Refresh_Token_Successfully()
    {
        var userId = "test@bs3.dev";
        var password = "123456";
        var ip = "127.0.0.1";
        var expiry = 60000;

        // Create token
        var created = await _tokenAppService.CreateAsync(userId, password,  expiry, ip, CancellationToken.None);
        created.Status.Should().Be(TokenStatusConst.Authorized);
        created.AccessToken.Should().NotBeNull();

        // Refresh token
        var refreshed = await _tokenAppService.RefreshAsync(created.Id, userId, expiry, ip, CancellationToken.None);
        refreshed.Status.Should().Be(TokenStatusConst.Authorized);
        refreshed.AccessToken.Should().NotBe(created.AccessToken);
    }
}