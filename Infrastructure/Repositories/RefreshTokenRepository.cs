using Microsoft.EntityFrameworkCore;
using TimeShop.Domain.Entities;
using TimeShop.Domain.Interfaces;
using TimeShop.Infrastructure.Data;

namespace TimeShop.Infrastructure.Repositories
{
    public class RefreshTokenRepository : Repository<RefreshTokenEntity>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<RefreshTokenEntity?> GetByToken(string token)
        {
            return await _context.Set<RefreshTokenEntity>().FirstOrDefaultAsync(x => x.Token == token);
        }

        public async Task<RefreshTokenEntity?> GetByUserId(int userId)
        {
            return await _context.Set<RefreshTokenEntity>().FirstOrDefaultAsync(x => x.UserId == userId);
        }
    }
}
