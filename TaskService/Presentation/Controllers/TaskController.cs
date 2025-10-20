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
            if (id <= 0)
                return BadRequest(new ProblemDetails { Status = 400, Detail = "Invalid Task ID" });

            try
            {
                var task = await _taskApplicationService.GetByIdAsync(id);
                return Ok(task);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ProblemDetails { Status = 404, Detail = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Detail = $"Unexpected error occured {ex.Message}" });
            }
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllTasks()
        {
            try
            {
                var tasks = await _taskApplicationService.GetAllAsync();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Detail = $"Unexpected error occured {ex.Message}" });
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetTasksByUser()
        {
            try
            {
                var userId = _userApplicationService.GetCurrentUserId(User);
                var tasks = await _taskApplicationService.GetByUserIdAsync(userId);
                return Ok(tasks);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ProblemDetails { Status = 401, Detail = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Detail = $"Unexpected error occured {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            if (createTaskDto == null)
                return BadRequest(new ProblemDetails { Status = 400, Detail = "Request body cannot be null" });
            if (!ModelState.IsValid)
                return BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Detail = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                });

            try
            {
                var userId = _userApplicationService.GetCurrentUserId(User);
                var task = await _taskApplicationService.AddAsync(createTaskDto, userId);
                return CreatedAtAction(nameof(GetTaskById), new { task.Id }, task);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails { Status = 400, Detail = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ProblemDetails { Status = 401, Detail = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Detail = $"Unexpected error occured {ex.Message}" });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateTask([FromBody] UpdateTaskDto updateTaskDto)
        {
            if (updateTaskDto == null)
                return BadRequest(new ProblemDetails { Status = 400, Detail = "Request body cannot be null" });
            if (!ModelState.IsValid)
                return BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Detail = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))
                });

            try
            {
                var userId = _userApplicationService.GetCurrentUserId(User);
                await _taskApplicationService.UpdateAsync(updateTaskDto, userId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ProblemDetails { Status = 400, Detail = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ProblemDetails { Status = 404, Detail = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ProblemDetails { Status = 401, Detail = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Detail = $"Unexpected error occured {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            if (id <= 0)
                return BadRequest(new ProblemDetails { Status = 400, Detail = "Invalid Task ID" });

            try
            {
                var userId = _userApplicationService.GetCurrentUserId(User);
                await _taskApplicationService.DeleteAsync(id, userId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ProblemDetails { Status = 404, Detail = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new ProblemDetails { Status = 401, Detail = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Detail = $"Unexpected error occured {ex.Message}" });
            }
        }

    }
}
