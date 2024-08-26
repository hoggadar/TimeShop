using TimeShop.Application.Interfaces;
using TimeShop.Domain.Entities;
using TimeShop.Domain.Interfaces;

namespace TimeShop.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<RoleEntity?> GetById(int id)
        {
            return await _roleRepository.GetById(id);
        }

        public async Task<RoleEntity?> GetByName(string name)
        {
            return await _roleRepository.GetByName(name);
        }
    }
}
