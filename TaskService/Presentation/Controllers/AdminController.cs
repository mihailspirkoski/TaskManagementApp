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
            await _userApplicationService.ChangeRoleAsync(userId, role);
            return NoContent();
        }
    }
}
