using JobTracker.Application.DTOs.AuthDtos.ChangePasswordDtos;
using JobTracker.Application.DTOs.AuthDtos.LoginDtos;
using JobTracker.Application.DTOs.AuthDtos.RefreshTokenDtos;
using JobTracker.Application.DTOs.AuthDtos.RegisterDtos;
using JobTracker.Application.DTOs.AuthDtos.UserDtos;
using JobTracker.Application.Interfaces.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto dto)
        {
            var result = await _userService.RegisterAsync(dto);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto)
        {
            var result = await _userService.LoginAsync(dto);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<ActionResult<LoginResponseDto>> Refresh(RefreshTokenRequestDto dto)
        {
            var result = await _userService.RefreshAsync(dto);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            await _userService.ChangePasswordAsync(dto);
            return NoContent();
        }
    }
}
