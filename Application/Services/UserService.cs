using TimeShop.Application.DTOs.User;
using TimeShop.Application.Interfaces;
using TimeShop.Domain.Entities;
using TimeShop.Domain.Interfaces;

namespace TimeShop.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public UserService(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        public async Task<List<UserEntity>> GetAll()
        {
            return await _userRepository.GetAll();
        }

        public async Task<UserEntity?> GetById(int id)
        {
            return await _userRepository.GetById(id);
        }

        public async Task<UserEntity?> GetByEmail(string email)
        {
            return await _userRepository.GetByEmail(email);
        }

        public async Task<UserEntity?> Create(SignupDTO dto, string roleName)
        {
            var role = await _roleRepository.GetByName(roleName);
            if (role == null) return null;
            var newUser = new UserEntity
            {
                Name = dto.Name,
                Surname = dto.Surname,
                Email = dto.Email,
                Password = HashPassword(dto.Password),
                RoleId = role.Id,
            };
            await _userRepository.Create(newUser);
            return newUser;
        }

        public async Task<UserEntity?> Update(int id, UpdateUserDTO dto)
        {
            var user = await _userRepository.GetById(id);
            if (user == null) return null;
            if (!VerifyPassword(user.Password, dto.Password)) return null;
            user.Name = dto.Name;
            user.Surname = dto.Surname;
            user.Email = dto.Email;
            await _userRepository.Update(user);
            return user;
        }

        public async Task<bool> Delete(int id)
        {
            var user = await _userRepository.GetById(id);
            if (user == null) return false;
            await _userRepository.Delete(user);
            return true;
        }

        public bool VerifyPassword(string userPassword, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, userPassword);
        }

        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
