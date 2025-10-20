using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Core.Enums;
using TaskService.Application;
using TaskService.Application.DTOs;

namespace TaskService.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly IUserApplicationService _userApplicationService;

        public AdminController(IUserApplicationService userApplicationService)
        {
            _userApplicationService = userApplicationService;
        }

        [HttpPut("users/{userId}/{role}")]
        public async Task<IActionResult> ChangeUserRole(int userId, UserRole role)
        {
            if(userId <= 0 )
                return BadRequest(new ProblemDetails { Status = 400, Detail = "Invalid user ID" });

            try
            {
                await _userApplicationService.ChangeRoleAsync(userId, role);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ProblemDetails { Status = 404, Detail = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails { Status = 400, Detail = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Detail = $"Unexpected error occured {ex.Message}" });
            }
        }
    }
}
