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
            if(userRegisterDto == null)
            {
                return BadRequest(new ProblemDetails { Status = 400, Detail = "Request body is null" });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemDetails { Status = 400, Detail = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)) });
            }

            try
            {
                var token = await _userApplicationService.RegisterAsync(userRegisterDto);
                return Ok(new { Token = token });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails { Status = 400, Detail = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ProblemDetails { Status = 400, Detail = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Detail = $"Unexpected error - {ex.Message}" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto userLoginDto)
        {
            if (userLoginDto == null)
            {
                return BadRequest(new ProblemDetails { Status = 400, Detail = "Request body is null" });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new ProblemDetails { Status = 400, Detail = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)) });
            }

            try
            {
                var token = await _userApplicationService.LoginAsync(userLoginDto);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ProblemDetails { Status = 401, Detail = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Detail = $"Unexpected error - {ex.Message}" });
            }
        }
    }
}