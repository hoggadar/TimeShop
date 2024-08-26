using TimeShop.Application.DTOs.User;
using TimeShop.Domain.Entities;

namespace TimeShop.Application.Interfaces
{
    public interface IUserService
    {
        Task<List<UserEntity>> GetAll();
        Task<UserEntity?> GetById(int id);
        Task<UserEntity?> GetByEmail(string email);
        Task<UserEntity?> Create(SignupDTO dto);
        Task<UserEntity?> Update(int id, UpdateUserDTO dto);
        Task<bool> Delete(int id);
    }
}
