using Microsoft.EntityFrameworkCore;
using TimeShop.Domain.Entities;
using TimeShop.Domain.Interfaces;
using TimeShop.Infrastructure.Data;

namespace TimeShop.Infrastructure.Repositories
{
    public class UserRepository : Repository<UserEntity>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<UserEntity?> GetByEmail(string email)
        {
            return await _context.Set<UserEntity>().FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}
