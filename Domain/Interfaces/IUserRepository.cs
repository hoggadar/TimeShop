using TimeShop.Domain.Entities;

namespace TimeShop.Domain.Interfaces
{
    public interface IUserRepository : IRepository<UserEntity>
    {
        Task<UserEntity?> GetByEmail(string email);
    }
}
