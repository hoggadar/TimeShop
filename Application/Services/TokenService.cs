using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TimeShop.Application.DTOs.User;
using TimeShop.Application.Interfaces;
using TimeShop.Domain.Entities;
using TimeShop.Domain.Interfaces;
using TimeShop.Migrations;

namespace TimeShop.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserService _userService;

        public TokenService(IConfiguration configuration, IRefreshTokenRepository refreshTokenRepository, IUserService userService)
        {
            _configuration = configuration;
            _refreshTokenRepository = refreshTokenRepository;
            _userService = userService;
        }

        public async Task<RefreshTokenEntity?> GetRefreshTokenByToken(string token)
        {
            return await _refreshTokenRepository.GetByToken(token);
        }

        public async Task<RefreshTokenEntity?> GetRefreshTokenByUserId(int userId)
        {
            return await _refreshTokenRepository.GetByUserId(userId);
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenOptions = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(1),
                signingCredentials: creds
            );
            string token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return token;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<RefreshTokenEntity?> CreateRefreshToken(int userId)
        {
            var user = await _userService.GetById(userId);
            if (user == null) return null;
            var token = new RefreshTokenEntity
            {
                Token = GenerateRefreshToken(),
                ExpiryTime = DateTime.UtcNow.AddMinutes(5),
                Revoked = false,
                UserId = userId
            };
            await _refreshTokenRepository.Create(token);
            return token;
        }

        public async Task DeleteRefreshToken(RefreshTokenEntity token)
        {
            await _refreshTokenRepository.Delete(token);
        }

        public async Task RevokeRefreshTokenByUserId(int userId)
        {
            var token = await GetRefreshTokenByUserId(userId);
            if (token != null)
            {
                token.Revoked = true;
                await _refreshTokenRepository.Update(token);
            }
        }

        public async Task ActivateRefreshTokenByUserId(int userId)
        {
            var token = await GetRefreshTokenByUserId(userId);
            if (token != null)
            {
                token.Revoked = false;
                await _refreshTokenRepository.Update(token);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredAccessToken(string token)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = key,
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
                var jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }
                return principal;
            }
            catch (SecurityTokenException stex)
            {
                throw new SecurityTokenException("The token is invalid or has expired", stex);
            }
        }

        public async Task<UserEntity?> GetUserFromExpiredAccessToken(string token)
        {
            ClaimsPrincipal principal = GetPrincipalFromExpiredAccessToken(token);
            Claim emailClaim = principal.Claims.First(x => x.Type == ClaimTypes.Email);
            var user = await _userService.GetByEmail(emailClaim.Value);
            return user;
        }

        public bool RefreshTokenIsExpired(RefreshTokenEntity token)
        {
            return DateTime.UtcNow >= token.ExpiryTime;
        }

        public async Task<bool> VerifyRefreshToken(TokenDTO dto)
        {
            var user = await GetUserFromExpiredAccessToken(dto.AccessToken);
            if (user == null) return false;
            var refreshToken = await GetRefreshTokenByToken(dto.RefreshToken);
            return !(refreshToken == null || user.Id != refreshToken.UserId || RefreshTokenIsExpired(refreshToken));
        }
    }
}
