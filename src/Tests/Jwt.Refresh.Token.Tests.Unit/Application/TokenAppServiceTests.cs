using FluentAssertions;
using Jwt.Refresh.Token.Application;
using Jwt.Refresh.Token.Application.Interfaces;
using Jwt.Refresh.Token.Domain.Constants;
using Jwt.Refresh.Token.Domain.Entities;
using Jwt.Refresh.Token.Domain.Entities.Repositories;
using Jwt.Refresh.Token.Domain.Extensions;
using Jwt.Refresh.Token.Domain.Services.Interfaces;
using Moq;

namespace Jwt.Refresh.Token.Tests.Unit.Application;

public class TokenAppServiceTests
{
    private readonly Mock<ITokenRepository> _tokenRepositoryMock;
    private readonly Mock<ICredentialRepository> _credentialRepository;
    private readonly Mock<IJwtAccessTokenService> _jwtAccessTokenServiceMock;

    private readonly ITokenAppService _tokenAppService;

    private readonly string _userId;
    private readonly string _password;
    private readonly string _ipAddress;

    public TokenAppServiceTests()
    {
        _tokenRepositoryMock = new Mock<ITokenRepository> { };
        _credentialRepository = new Mock<ICredentialRepository> { };
        _jwtAccessTokenServiceMock = new Mock<IJwtAccessTokenService> { };

        _tokenAppService = new TokenAppService(_tokenRepositoryMock.Object,
            _credentialRepository.Object, _jwtAccessTokenServiceMock.Object);

        _userId = "test@bs3.dev";
        _password = "password_test";
        _ipAddress = "127.0.0.1";
    }

    [Fact]
    public async Task Create_Token_ReturnsAuhtorized()
    {
        var cancellationToken = CancellationToken.None;

        _credentialRepository.Setup(x => x.GetAsync(_userId, _password, cancellationToken))
            .ReturnsAsync(_userId);

        _jwtAccessTokenServiceMock.Setup(x => x.GetAsync(_userId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("new_access_token");

        _tokenRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<TokenEntity>(), cancellationToken));

        var token = await _tokenAppService.CreateAsync(_userId, _password, It.IsAny<int>(), _ipAddress,
            It.IsAny<CancellationToken>());

        token.Status
            .Should()
            .Be(TokenStatusConst.Authorized);
    }

    [Fact]
    public async Task Create_Token_UserIdOrPasswordInvalid_ReturnsUnauhtorized()
    {
        var token = await _tokenAppService.CreateAsync(string.Empty, string.Empty, 1.MinutesToMilliseconds(),
            _ipAddress,
            CancellationToken.None);

        token.Status.Should().Be(TokenStatusConst.Unauthorized);
    }

    [Fact]
    public async Task Create_Token_UserIdNotFound_ReturnsUnauhtorized()
    {
        _credentialRepository.Setup(x => x.GetAsync(_userId, _password, CancellationToken.None))
            .ReturnsAsync(string.Empty);

        var token = await _tokenAppService.CreateAsync(_userId, _password, 1.MinutesToMilliseconds(), _ipAddress,
            CancellationToken.None);

        token.Status.Should().Be(TokenStatusConst.Unauthorized);
    }

    [Fact]
    public async Task Create_Token_UnexpectedError_ReturnsError()
    {
        _credentialRepository.Setup(x => x.GetAsync(_userId, _password, CancellationToken.None))
            .ThrowsAsync(new Exception("Unexpected error"));

        var token = await _tokenAppService.CreateAsync(_userId, _password, 1.MinutesToMilliseconds(), _ipAddress,
            CancellationToken.None);

        token.Status.Should().Be(TokenStatusConst.Error);
    }

    [Fact]
    public async Task Refresh_Token_InvalidInput_ReturnsUnauthorized()
    {
        var result =
            await _tokenAppService.RefreshAsync(string.Empty, string.Empty, 1000, _ipAddress, CancellationToken.None);

        result.Status.Should().Be(TokenStatusConst.Unauthorized);
    }

    [Fact]
    public async Task Refresh_Token_NotFound_ReturnsUnauthorized()
    {
        _tokenRepositoryMock
            .Setup(x => x.GetAsync("5d4369a3-29b5-49fb-aaf9-c00d07284ec7", _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TokenEntity)null!);

        var result = await _tokenAppService.RefreshAsync("5d4369a3-29b5-49fb-aaf9-c00d07284ec7", _userId, 1000,
            _ipAddress, CancellationToken.None);

        result.Status.Should().Be(TokenStatusConst.Unauthorized);
    }

    [Fact]
    public async Task Refresh_Token_Success_ReturnsAuthorized()
    {
        var entity = new TokenEntity { Id = "5d4369a3-29b5-49fb-aaf9-c00d07284ec7", UserId = _userId };

        _tokenRepositoryMock
            .Setup(x => x.GetAsync("5d4369a3-29b5-49fb-aaf9-c00d07284ec7", _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _jwtAccessTokenServiceMock
            .Setup(x => x.GetAsync(_userId, 1000, It.IsAny<CancellationToken>()))
            .ReturnsAsync("new_access_token");

        _tokenRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<TokenEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var result = await _tokenAppService.RefreshAsync("5d4369a3-29b5-49fb-aaf9-c00d07284ec7", _userId, 1000,
            _ipAddress, CancellationToken.None);

        result.Status.Should().Be(TokenStatusConst.Authorized);
        result.AccessToken.Should().Be("new_access_token");
    }

    [Fact]
    public async Task Refresh_Token_ThrowsException_ReturnsError()
    {
        _tokenRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Some error"));

        var result = await _tokenAppService.RefreshAsync("5d4369a3-29b5-49fb-aaf9-c00d07284ec7", _userId, 1000,
            _ipAddress, CancellationToken.None);

        result.Status.Should().Be(TokenStatusConst.Error);
    }

    [Fact]
    public async Task TryRevoke_Token_NotFound_ReturnsZero()
    {
        _tokenRepositoryMock
            .Setup(x => x.GetAsync("5d4369a3-29b5-49fb-aaf9-c00d07284ec7", _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TokenEntity)null!);

        var result = await _tokenAppService.TryRevokeAsync("5d4369a3-29b5-49fb-aaf9-c00d07284ec7", _userId, _ipAddress,
            CancellationToken.None);

        result.Should().Be(0);
    }

    [Fact]
    public async Task TryRevoke_Token_Success_ReturnsOne()
    {
        var entity = new TokenEntity { Id = "5d4369a3-29b5-49fb-aaf9-c00d07284ec7", UserId = _userId };

        _tokenRepositoryMock
            .Setup(x => x.GetAsync("5d4369a3-29b5-49fb-aaf9-c00d07284ec7", _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        _tokenRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<TokenEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _tokenAppService.TryRevokeAsync("5d4369a3-29b5-49fb-aaf9-c00d07284ec7", _userId, _ipAddress,
            CancellationToken.None);

        result.Should().Be(1);
    }

    [Fact]
    public async Task TryRevoke_Token_ThrowsException_ReturnsZero()
    {
        _tokenRepositoryMock
            .Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Unexpected"));

        var result = await _tokenAppService.TryRevokeAsync("5d4369a3-29b5-49fb-aaf9-c00d07284ec7", _userId, _ipAddress,
            CancellationToken.None);

        result.Should().Be(0);
    }
}