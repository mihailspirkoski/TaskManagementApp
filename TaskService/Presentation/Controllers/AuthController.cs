using Microsoft.AspNetCore.Mvc;
using TaskService.Application;
using TaskService.Application.DTOs;

namespace TaskService.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IUserApplicationService _userApplicationService;
        public AuthController(IUserApplicationService userApplicationService) =>
             _userApplicationService = userApplicationService;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto userRegisterDto)
        {

            var token = await _userApplicationService.RegisterAsync(userRegisterDto);
            return Ok(new { Token = token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto userLoginDto)
        {
            var token = await _userApplicationService.LoginAsync(userLoginDto);
            return Ok(new { Token = token });
        }

    }
}

//[HttpPost("register")]
//public async Task<IActionResult> Register([FromBody] RegisterUserDto userRegisterDto)
//{
//    try
//    {
//        var token = await _userApplicationService.RegisterAsync(userRegisterDto);
//        return Ok(new { Token = token });
//    }
//    catch (ArgumentException ex)
//    {
//        return BadRequest(ex.Message);
//    }
//    catch (InvalidOperationException ex)
//    {
//        return BadRequest(ex.Message);
//    }
//}

//[HttpPost("login")]
//public async Task<IActionResult> Login([FromBody] LoginUserDto userLoginDto)
//{
//    try
//    {
//        var token = await _userApplicationService.LoginAsync(userLoginDto);
//        return Ok(new { Token = token });
//    }
//    catch (UnauthorizedAccessException ex)
//    {
//        return BadRequest(ex.Message);
//    }
//}