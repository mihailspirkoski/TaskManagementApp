using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskService.Application;
using TaskService.Application.DTOs;

namespace TaskService.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskApplicationService _taskApplicationService;
        private readonly IUserApplicationService _userApplicationService;

        public TaskController(ITaskApplicationService taskApplicationService, IUserApplicationService userApplicationService)
        {
            _taskApplicationService = taskApplicationService;
            _userApplicationService = userApplicationService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _taskApplicationService.GetByIdAsync(id);
            return Ok(task);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await _taskApplicationService.GetAllAsync();
            return Ok(tasks);
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetTasksByUser()
        {
            var userId = _userApplicationService.GetCurrentUserId(User);
            var tasks = await _taskApplicationService.GetByUserIdAsync(userId);
            return Ok(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            var userId = _userApplicationService.GetCurrentUserId(User);
            var task = await _taskApplicationService.AddAsync(createTaskDto, userId);
            return CreatedAtAction(nameof(GetTaskById), new { task.Id }, task);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskDto updateTaskDto)
        {
            var userId = _userApplicationService.GetCurrentUserId(User);
            await _taskApplicationService.UpdateAsync(updateTaskDto, userId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = _userApplicationService.GetCurrentUserId(User);
            await _taskApplicationService.DeleteAsync(id, userId);
            return NoContent();
        }

    }
}
