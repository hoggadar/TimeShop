using Microsoft.EntityFrameworkCore;
using TimeShop.Domain.Entities;
using TimeShop.Domain.Interfaces;
using TimeShop.Infrastructure.Data;

namespace TimeShop.Infrastructure.Repositories
{
    public class RoleRepository : Repository<RoleEntity>, IRoleRepository
    {
        public RoleRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<RoleEntity?> GetByName(string name)
        {
            return await _context.Set<RoleEntity>().FirstOrDefaultAsync(x => x.Name == name);
        }
    }
}
