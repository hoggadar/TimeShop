using System.Security.Claims;
using TimeShop.Application.DTOs.User;
using TimeShop.Domain.Entities;

namespace TimeShop.Application.Interfaces
{
    public interface ITokenService
    {
        Task<RefreshTokenEntity?> GetRefreshTokenByToken(string token);
        Task<RefreshTokenEntity?> GetRefreshTokenByUserId(int userId);
        string GenerateAccessToken(IEnumerable<Claim> claims);
        string GenerateRefreshToken();
        Task<RefreshTokenEntity?> CreateRefreshToken(int userId);
        Task DeleteRefreshToken(RefreshTokenEntity token);
        Task RevokeRefreshTokenByUserId(int userId);
        Task ActivateRefreshTokenByUserId(int userId);
        ClaimsPrincipal GetPrincipalFromExpiredAccessToken(string token);
        Task<UserEntity?> GetUserFromExpiredAccessToken(string token);
        bool RefreshTokenIsExpired(RefreshTokenEntity token);
        Task<bool> VerifyRefreshToken(TokenDTO dto);
    }
}
