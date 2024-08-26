using TimeShop.Domain.Entities;

namespace TimeShop.Application.Interfaces
{
    public interface IRoleService
    {
        Task<RoleEntity?> GetById(int id);
        Task<RoleEntity?> GetByName(string name);
    }
}
