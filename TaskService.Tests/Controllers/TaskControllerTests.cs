using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using TaskService.Application;
using TaskService.Application.DTOs;
using TaskService.Presentation.Controllers;

namespace TaskService.Tests.Controllers
{
    public class TaskControllerTests
    {
        private readonly Mock<ITaskApplicationService> _mockTaskApplicationService;
        private readonly Mock<IUserApplicationService> _mockUserApplicationService;
        private readonly TaskController _controller;

        public TaskControllerTests()
        {
            _mockTaskApplicationService = new Mock<ITaskApplicationService>();
            _mockUserApplicationService = new Mock<IUserApplicationService>();
            _controller = new TaskController(_mockTaskApplicationService.Object, _mockUserApplicationService.Object);
        }

        [Fact]
        public async Task GetTaskById_ReturnsOk_WhenTaskExists()
        {
            // Arrange
            var taskId = 1;
            var task = new Shared.Core.Entities.Task { Id = taskId, Title = "Test Task", UserId = 1 };
            _mockTaskApplicationService.Setup(s => s.GetByIdAsync(taskId)).ReturnsAsync(task);

            // Act
            var result = await _controller.GetTaskById(taskId);

            // Assert
            result.Should().BeOfType<OkObjectResult>(because: "Existing task should return OK 200");
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(task);
            _mockTaskApplicationService.Verify(s => s.GetByIdAsync(taskId), Times.Once);
        }

        [Fact]
        public async Task GetTaskById_ReturnsNotFound_WhenTaskNotFound()
        {
            // Arrange
            int taksId = 999;
            _mockTaskApplicationService.Setup(s => s.GetByIdAsync(taksId))
                .ThrowsAsync(new KeyNotFoundException("Task not found"));

            // Act
            var result = await _controller.GetTaskById(taksId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>(because: "Non-existing task should return NotFound 404");
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.As<ProblemDetails>().Detail.Should().Contain("Task not found");
            _mockTaskApplicationService.Verify(s => s.GetByIdAsync(taksId), Times.Once);
        }

        [Fact]
        public async Task GetTaskById_ReturnsBadRequest_WhenIdIsInvalid()
        {
            // Arrange
            var taskId = 0;

            // Act
            var result = await _controller.GetTaskById(taskId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>(because: "Invalid task ID should return BadRequest 400");
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.As<ProblemDetails>().Detail.Should().Contain("Invalid Task ID");
            _mockTaskApplicationService.Verify(s => s.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetAllTasks_ReturnsOk_WhenTasksExist()
        {
            // Arrange
            var tasks = new List<Shared.Core.Entities.Task>
            {
                new Shared.Core.Entities.Task { Id = 1, Title = "Task 1", UserId = 1 },
                new Shared.Core.Entities.Task { Id = 2, Title = "Task 2", UserId = 1 }
            };
            _mockTaskApplicationService.Setup(s => s.GetAllAsync()).ReturnsAsync(tasks);

            // Act
            var result = await _controller.GetAllTasks();

            // Assert
            result.Should().BeOfType<OkObjectResult>(because: "Existing tasks should return OK 200");
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(tasks);
            _mockTaskApplicationService.Verify(s => s.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetTasksByUser_ReturnsOk_WhenTasksExist()
        {
            // Arrange
            var userId = 1;
            var tasks = new List<Shared.Core.Entities.Task>
            {
                new Shared.Core.Entities.Task { Id = 1, Title = "Task 1", UserId = userId },
                new Shared.Core.Entities.Task { Id = 2, Title = "Task 2", UserId = userId }
            };
            _mockUserApplicationService.Setup(s => s.GetCurrentUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns(userId);
            _mockTaskApplicationService.Setup(s => s.GetByUserIdAsync(userId)).ReturnsAsync(tasks);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
                    {
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, userId.ToString())
                    }))
                }
            };

            // Act
            var result = await _controller.GetTasksByUser();

            // Assert
            result.Should().BeOfType<OkObjectResult>(because: "User with tasks should return OK 200");
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(tasks);
            _mockUserApplicationService.Verify(s => s.GetCurrentUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()), Times.Once);
            _mockTaskApplicationService.Verify(s => s.GetByUserIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetTasksByUser_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            _mockUserApplicationService.Setup(s => s.GetCurrentUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .Throws(new UnauthorizedAccessException("User is not authenticated"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity())
                }
            };

            // Act
            var result = await _controller.GetTasksByUser();

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>(because: "Unauthenticated user should return 401 Unauthorized");
            var unauthorizedResult = result as UnauthorizedObjectResult;
            unauthorizedResult.Should().NotBeNull();
            unauthorizedResult.StatusCode.Should().Be(401);
            unauthorizedResult.Value.As<ProblemDetails>().Detail.Should().Contain("User is not authenticated");
            _mockUserApplicationService.Verify(s => s.GetCurrentUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()), Times.Once);
            _mockTaskApplicationService.Verify(s => s.GetByUserIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task CreateTask_ReturnsCreated_WhenTaskIsCreated()
        {
            int userId = 1;
            var createTaskDto = new CreateTaskDto(
                "New Task",
                "Task Description",
                DateTime.UtcNow.AddDays(7)
            );
            var task = new Shared.Core.Entities.Task
            {
                Id = 1,
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                DueDate = createTaskDto.DueDate,
                UserId = userId
            };
            _mockUserApplicationService.Setup(s => s.GetCurrentUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns(userId);
            _mockTaskApplicationService.Setup(s => s.AddAsync(createTaskDto, userId)).ReturnsAsync(task);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
                    {
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, userId.ToString())
                    }))
                }
            };

            // Act
            var result = await _controller.CreateTask(createTaskDto);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>(because: "Successful creation should return 201 Created");
            var createdResult = result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult.StatusCode.Should().Be(201);
            createdResult.ActionName.Should().Be(nameof(_controller.GetTaskById));
            createdResult.RouteValues.Should().NotBeNull();
            createdResult.RouteValues["id"].Should().Be(task.Id);
            createdResult.Value.Should().BeEquivalentTo(task);
            _mockUserApplicationService.Verify(s => s.GetCurrentUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()), Times.Once);
            _mockTaskApplicationService.Verify(s => s.AddAsync(createTaskDto, userId), Times.Once);
        }

        [Fact]
        public async Task CreateTask_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Arrange

            // Act
            var result = await _controller.CreateTask(null!);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>(because: "Null dto should return 400 Bad Request");
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.As<ProblemDetails>().Detail.Should().Contain("Request body cannot be null");
            _mockUserApplicationService.Verify(s => s.GetCurrentUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()), Times.Never);
            _mockTaskApplicationService.Verify(s => s.AddAsync(It.IsAny<CreateTaskDto>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task CreateTask_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var createTaskDto = new CreateTaskDto(
                "",
                "Task Description",
                DateTime.UtcNow.AddDays(7)
            );
            _controller.ModelState.AddModelError("Title", "The Title field is required.");

            // Act
            var result = await _controller.CreateTask(createTaskDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>(because: "Invalid model state should return 400 Bad Request");
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.As<ProblemDetails>().Detail.Should().Contain("The Title field is required.");
            _mockUserApplicationService.Verify(s => s.GetCurrentUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()), Times.Never);
            _mockTaskApplicationService.Verify(s => s.AddAsync(It.IsAny<CreateTaskDto>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task CreateTask_ReturnsUnauthorized_WhenUserNotAuthenticated()
        {
            // Arrange
            var createTaskDto = new CreateTaskDto(
                "New Task",
                "Task Description",
                DateTime.UtcNow.AddDays(7)
            );
            _mockUserApplicationService.Setup(s => s.GetCurrentUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .Throws(new UnauthorizedAccessException("User is not authenticated"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity())
                }
            };

            // Act
            var result = await _controller.CreateTask(createTaskDto);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>(because: "Unauthenticated user should return 401 Unauthorized");
            var unauthorizedResult = result as UnauthorizedObjectResult;
            unauthorizedResult.Should().NotBeNull();
            unauthorizedResult.StatusCode.Should().Be(401);
            unauthorizedResult.Value.As<ProblemDetails>().Detail.Should().Contain("User is not authenticated");
            _mockUserApplicationService.Verify(s => s.GetCurrentUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>()), Times.Once);
            _mockTaskApplicationService.Verify(s => s.AddAsync(It.IsAny<CreateTaskDto>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task UpdateTask_ReturnsNoContent_WhenTaskIsUpdated()
        {

            // Arrange
            int userId = 1;
            var updateTaskDto = new UpdateTaskDto(
                1,
                "Updated Task",
                "Updated Description",
                DateTime.UtcNow.AddDays(10),
                true
            );
            _mockUserApplicationService.Setup(s => s.GetCurrentUserId(It.IsAny<System.Security.Claims.ClaimsPrincipal>())).Returns(userId);
            _mockTaskApplicationService.Setup(s => s.UpdateAsync(updateTaskDto, userId)).Returns(Task.CompletedTask);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
                    {
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, userId.ToString())
                    }))
                }
            };

            // Act
            var result = await _controller.UpdateTask(updateTaskDto);

            // Assert
            result.Should().BeOfType<NoContentResult>(because: "Successful update should return 204 No Content");
            var noContentResult = result as NoContentResult;
            noContentResult.Should().NotBeNull();
            noContentResult.StatusCode.Should().Be(204);
            _mockUserApplicationService.Verify(s => s.GetCurrentUserId(It.IsAny<ClaimsPrincipal>()), Times.Once());
            _mockTaskApplicationService.Verify(s => s.UpdateAsync(updateTaskDto, userId), Times.Once);
        }

        [Fact]
        public async Task UpdateTask_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = await _controller.UpdateTask(null!);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>(because: "Null dto should return 400 Bad Request");
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.As<ProblemDetails>().Detail.Should().Contain("Request body cannot be null");
            _mockUserApplicationService.Verify(s => s.GetCurrentUserId(It.IsAny<ClaimsPrincipal>()), Times.Never());
            _mockTaskApplicationService.Verify(s => s.UpdateAsync(It.IsAny<UpdateTaskDto>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task UpdateTask_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var updateTaskDto = new UpdateTaskDto(
                1,
                "",
                "Updated Description",
                DateTime.UtcNow.AddDays(10),
                true
            );
            _controller.ModelState.AddModelError("Title", "The Title field is required.");

            // Act
            var result = await _controller.UpdateTask(updateTaskDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>(because: "Invalid model state should return 400 Bad Request");
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.As<ProblemDetails>().Detail.Should().Contain("The Title field is required.");
            _mockUserApplicationService.Verify(s => s.GetCurrentUserId(It.IsAny<ClaimsPrincipal>()), Times.Never());
            _mockTaskApplicationService.Verify(s => s.UpdateAsync(It.IsAny<UpdateTaskDto>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task UpdateTask_ReturnsUnauthorized_WhenUserLacksPermission()
        {
            // Arrange
            int userId = 1;
            var updateTaskDto = new UpdateTaskDto(
                1,
                "Updated Task",
                "Updated Description",
                DateTime.UtcNow.AddDays(10),
                true
            );
            _mockUserApplicationService.Setup(s => s.GetCurrentUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            _mockUserApplicationService.Setup(s => s.GetCurrentUserId(It.IsAny<ClaimsPrincipal>()))
                .Throws(new UnauthorizedAccessException("You do not have permission to update this task"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
                    {
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, userId.ToString())
                    }))
                }
            };

            // Act
            var result = await _controller.UpdateTask(updateTaskDto);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>(because: "Unauthenticated user should return 401 Unauthorized");
            var unauthorizedResult = result as UnauthorizedObjectResult;
            unauthorizedResult.Should().NotBeNull();
            unauthorizedResult.StatusCode.Should().Be(401);
            unauthorizedResult.Value.As<ProblemDetails>().Detail.Should().Contain("You do not have permission to update this task");
            _mockUserApplicationService.Verify(s => s.GetCurrentUserId(It.IsAny<ClaimsPrincipal>()), Times.Once());
            _mockTaskApplicationService.Verify(s => s.UpdateAsync(It.IsAny<UpdateTaskDto>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task DeleteTask_ReturnsNoContent_WhenDeleteIsSuccessful()
        {

            // Arrange
            int userId = 1;
            int taskId = 1;
            _mockUserApplicationService.Setup(s => s.GetCurrentUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            _mockTaskApplicationService.Setup(s => s.DeleteAsync(taskId, userId)).Returns(Task.CompletedTask);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
                    {
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, userId.ToString())
                    }))
                }
            };

            // Act
            var result = await _controller.DeleteTask(taskId);

            // Assert
            result.Should().BeOfType<NoContentResult>(because: "Successful deletion should return 204 No Content");
            var noContentResult = result as NoContentResult;
            noContentResult.Should().NotBeNull();
            noContentResult.StatusCode.Should().Be(204);
            _mockUserApplicationService.Verify(s => s.GetCurrentUserId(It.IsAny<ClaimsPrincipal>()), Times.Once());
            _mockTaskApplicationService.Verify(s => s.DeleteAsync(taskId, userId), Times.Once);
        }

        [Fact]
        public async Task DeleteTask_ReturnsBadRequest_WhenIdIsInvalid()
        {

            // Arrange
            var taskId = 0;

            // Act
            var result = await _controller.DeleteTask(taskId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>(because: "Invalid task ID should return BadRequest 400");
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.As<ProblemDetails>().Detail.Should().Contain("Invalid Task ID");
            _mockUserApplicationService.Verify(s => s.GetCurrentUserId(It.IsAny<ClaimsPrincipal>()), Times.Never());
            _mockTaskApplicationService.Verify(s => s.DeleteAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteTask_ReturnsNotFound_WhenTaskNotFound()
        {
            // Arrange
            int userId = 1;
            int taskId = 999;
            _mockUserApplicationService.Setup(s => s.GetCurrentUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            _mockTaskApplicationService.Setup(s => s.DeleteAsync(taskId, userId))
                .ThrowsAsync(new KeyNotFoundException("Task not found"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
                    {
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, userId.ToString())
                    }))
                }
            };

            // Act
            var result = await _controller.DeleteTask(taskId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>(because: "Non-existent task should return 404 NotFound");
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult.StatusCode.Should().Be(404);
            notFoundResult.Value.As<ProblemDetails>().Detail.Should().Contain("Task not found");
            _mockUserApplicationService.Verify(s => s.GetCurrentUserId(It.IsAny<ClaimsPrincipal>()), Times.Once());
            _mockTaskApplicationService.Verify(s => s.DeleteAsync(taskId, userId), Times.Once());
        }

        [Fact]
        public async Task DeleteTask_ReturnsUnauthorized_WhenUserLacksPermission()
        {
            // Arrange
            int userId = 1;
            int taskId = 1;
            _mockUserApplicationService.Setup(s => s.GetCurrentUserId(It.IsAny<ClaimsPrincipal>())).Returns(userId);
            _mockTaskApplicationService.Setup(s => s.DeleteAsync(taskId, userId))
                .ThrowsAsync(new UnauthorizedAccessException("You do not have permission to delete this task"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(new[]
                    {
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, userId.ToString())
                    }))
                }
            };

            // Act
            var result = await _controller.DeleteTask(taskId);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>(because: "Unauthenticated user should return 401 Unauthorized");
            var unauthorizedResult = result as UnauthorizedObjectResult;
            unauthorizedResult.Should().NotBeNull();
            unauthorizedResult.StatusCode.Should().Be(401);
            unauthorizedResult.Value.As<ProblemDetails>().Detail.Should().Contain("You do not have permission to delete this task");
            _mockUserApplicationService.Verify(s => s.GetCurrentUserId(It.IsAny<ClaimsPrincipal>()), Times.Once());
            _mockTaskApplicationService.Verify(s => s.DeleteAsync(taskId, userId), Times.Once());
        }
    }
}
