using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeShop.Application.DTOs.User;
using TimeShop.Application.Interfaces;
using TimeShop.Application.Services;

namespace TimeShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IUserService _userServices;
        private readonly ITokenService _tokenService;

        public AuthenticationController(IRoleService roleService, IUserService userService, ITokenService tokenService)
        {
            _roleService = roleService;
            _userServices = userService;
            _tokenService = tokenService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupDTO dto)
        {
            var user = await _userServices.GetByEmail(dto.Email);
            if (user != null) return BadRequest("User already exists");
            var createdUser = await _userServices.Create(dto, "User");
            if (createdUser == null) return StatusCode(500, "Failed to create user");
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, createdUser.Email),
                new Claim(ClaimTypes.Role, createdUser.Role!.Name)
            };
            string token = _tokenService.GenerateAccessToken(claims);
            return Ok(token);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var user = await _userServices.GetByEmail(dto.Email);
            if (user == null || !_userServices.VerifyPassword(user.Password, dto.Password))
            {
                return Unauthorized("Incorrect email or password");
            }
            var role = await _roleService.GetById(user.RoleId);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role!.Name)
            };
            string token = _tokenService.GenerateAccessToken(claims);
            return Ok(token);
        }
    }
}
