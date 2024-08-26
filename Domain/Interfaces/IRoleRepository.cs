using TimeShop.Domain.Entities;

namespace TimeShop.Domain.Interfaces
{
    public interface IRoleRepository : IRepository<RoleEntity>
    {
        Task<RoleEntity?> GetByName(string name);
    }
}
