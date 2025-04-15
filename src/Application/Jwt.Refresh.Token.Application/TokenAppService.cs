using Jwt.Refresh.Token.Application.Interfaces;
using Jwt.Refresh.Token.Domain.Configs;
using Jwt.Refresh.Token.Domain.Constants;
using Jwt.Refresh.Token.Domain.Entities.Repositories;
using Jwt.Refresh.Token.Domain.Extensions;
using Jwt.Refresh.Token.Domain.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Jwt.Refresh.Token.Application;

public class TokenAppService : ITokenAppService
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IJwtAccessTokenService _jwtAccessTokenService;

        public TokenAppService(
            ITokenRepository tokenRepository, 
            IUserRepository userRepository,
            IJwtAccessTokenService jwtAccessTokenService)
        {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _jwtAccessTokenService = jwtAccessTokenService;
        }

        public async Task<Domain.DataTransferObjects.Token> CreateAsync(string userId, string password,
            int expiresMilliseconds, string ipAddress, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(password))
            {
                return new Domain.DataTransferObjects.Token
                {
                    UserId = userId, 
                    Status = TokenStatusConst.Unauthorized
                }; 
            }
                
            try
            {
                var entityUserId = await _userRepository.GetAsync(userId, password, cancellationToken);

                if (string.IsNullOrEmpty(entityUserId))
                {
                    return new Domain.DataTransferObjects.Token { UserId = userId, Status = TokenStatusConst.Unauthorized }; 
                }
                   
                var accessToken = await _jwtAccessTokenService.GetAsync(userId, expiresMilliseconds, cancellationToken);

                var token = new Domain.DataTransferObjects.Token
                {
                    Id = Guid.NewGuid().ToString(), 
                    UserId = userId,
                    AccessToken = accessToken,
                    Status = TokenStatusConst.Authorized,
                    Expires = expiresMilliseconds.ToDateTimeOffset()
                };

                await _tokenRepository.CreateAsync(token.ToEntity(ipAddress, expiresMilliseconds), cancellationToken);

                return token;
            }
            catch(Exception ex)
            {
                return new Domain.DataTransferObjects.Token { UserId = userId, Status = TokenStatusConst.Error };    
            }
        }

        public async Task<Domain.DataTransferObjects.Token> RefreshAsync(string tokenId, string userId,
            int expiresMilliseconds, string ipAddress, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(tokenId) || string.IsNullOrEmpty(userId))
            {
                return new Domain.DataTransferObjects.Token
                {
                    UserId = userId, 
                    Status = TokenStatusConst.Unauthorized
                }; 
            }

            try
            {
                var token = await _tokenRepository.GetAsync(tokenId, userId, cancellationToken);

                if (token is null)
                {
                    return new Domain.DataTransferObjects.Token 
                    {
                        UserId = userId, 
                        Status = TokenStatusConst.Unauthorized 
                    };  
                }

                var accessToken = await _jwtAccessTokenService.GetAsync(userId, expiresMilliseconds, cancellationToken);

                var newToken = new Domain.DataTransferObjects.Token
                {
                    Id = Guid.NewGuid().ToString(), 
                    UserId = userId, 
                    AccessToken = accessToken,
                    Status = TokenStatusConst.Authorized,
                    Expires = expiresMilliseconds.ToDateTimeOffset()
                };
                    
                await _tokenRepository.CreateAsync(newToken.ToEntity(ipAddress, expiresMilliseconds), cancellationToken);
                
                return newToken;
            }
            catch 
            {
                return new Domain.DataTransferObjects.Token { UserId = userId, Status = TokenStatusConst.Error };    
            }            
        }

        public async Task<int> TryRevokeAsync(string tokenId, string userId, string ipAddress, CancellationToken cancellationToken)
        {
            try
            {
                var token = await _tokenRepository.GetAsync(tokenId, userId, cancellationToken);
                if (token is null) return 0;
                
                return await _tokenRepository.UpdateAsync(token.ToRevokeTokenEntity(ipAddress), cancellationToken);
            }
            catch
            {
                return 0;
            }
        }
    }