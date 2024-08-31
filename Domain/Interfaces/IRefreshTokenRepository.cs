using TimeShop.Domain.Entities;

namespace TimeShop.Domain.Interfaces
{
    public interface IRefreshTokenRepository : IRepository<RefreshTokenEntity>
    {
        Task<RefreshTokenEntity?> GetByToken(string token);
        Task<RefreshTokenEntity?> GetByUserId(int userId);
    }
}
