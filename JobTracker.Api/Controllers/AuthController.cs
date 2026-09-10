using JobTracker.Application.DTOs.AuthDtos.RegisterDtos;
using JobTracker.Application.DTOs.AuthDtos.UserDtos;
using JobTracker.Application.Interfaces;
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
    }
}
