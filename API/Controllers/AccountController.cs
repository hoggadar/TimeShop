using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TimeShop.Application.Interfaces;
using TimeShop.Application.Services;

namespace TimeShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userServices;

        public AccountController(IUserService userServices)
        {
            _userServices = userServices;
        }

        [HttpGet("profile")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Profile()
        {
            var user = await _userServices.GetByEmail(User.FindFirst(ClaimTypes.Email)!.Value);
            return Ok(user);
        }
    }
}
