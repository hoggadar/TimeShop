using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeShop.Application.DTOs.User;
using TimeShop.Application.Interfaces;
using TimeShop.Domain.Entities;

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
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, createdUser.Email),
                new Claim(ClaimTypes.Role, createdUser.Role.Name)
            };
            string accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = await _tokenService.CreateRefreshToken(createdUser.Id);
            var tokenDTO = new TokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
            return Ok(tokenDTO);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var user = await _userServices.GetByEmail(dto.Email);
            if (user == null || !_userServices.VerifyPassword(user.Password, dto.Password))
            {
                return Unauthorized("Incorrect email or password");
            }
            var refreshToken = await _tokenService.GetRefreshTokenByUserId(user.Id);
            if (refreshToken != null && !_tokenService.RefreshTokenIsExpired(refreshToken) && refreshToken.Revoked)
            {
                await _tokenService.ActivateRefreshTokenByUserId(user.Id);
                return Ok("Refresh token is active");
            }
            await _tokenService.DeleteRefreshToken(refreshToken);
            var role = await _roleService.GetById(user.RoleId);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role.Name)
            };
            string accessToken = _tokenService.GenerateAccessToken(claims);
            refreshToken = await _tokenService.CreateRefreshToken(user.Id);
            var tokenDTO = new TokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token
            };
            return Ok(tokenDTO);
        }

        [HttpGet("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var user = await _userServices.GetByEmail(User.FindFirst(ClaimTypes.Email)!.Value);
            await _tokenService.RevokeRefreshTokenByUserId(user.Id);
            return Ok("Logout");
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenDTO dto)
        {
            var refreshToken = await _tokenService.GetRefreshTokenByToken(dto.RefreshToken);
            if (refreshToken == null || refreshToken.Revoked || _tokenService.RefreshTokenIsExpired(refreshToken))
            {
                return Unauthorized("Refresh token is incorrect or expired");
            }
            if (!await _tokenService.VerifyRefreshToken(dto))
            {
                return Unauthorized("Incorrect access or refresh token");
            }
            var user = await _userServices.GetById(refreshToken.UserId);
            var role = await _roleService.GetById(user.RoleId);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, role.Name)
            };
            string accessToken = _tokenService.GenerateAccessToken(claims);
            return Ok(accessToken);
        }
    }
}
