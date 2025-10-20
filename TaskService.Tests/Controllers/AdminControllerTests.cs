using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskService.Application;
using TaskService.Presentation.Controllers;

namespace TaskService.Tests.Controllers
{
    public class AdminControllerTests
    {
        private readonly Mock<IUserApplicationService> _mockUserApplicationService;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _mockUserApplicationService = new Mock<IUserApplicationService>();
            _controller = new AdminController(_mockUserApplicationService.Object);
        }

        [Fact]
        public async Task ChangeUserRole_ReturnsNoContent_WhenRoleChangeSucceds()
        {
            // Arrange
            int userId = 1;
            UserRole userRole = UserRole.Admin;
            _mockUserApplicationService.Setup(s => s.ChangeRoleAsync(userId, userRole))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ChangeUserRole(userId, userRole);

            // Assert
            result.Should().BeOfType<NoContentResult>(because: "Successful role change should return 204 NoContent");
            var noContentResult = result as NoContentResult;
            noContentResult.Should().NotBeNull(because: "Result should contain a valid response");
            noContentResult.StatusCode.Should().Be(204, because: "NoContentResult should have status code 204");
            _mockUserApplicationService.Verify(s => s.ChangeRoleAsync(userId, userRole), Times.Once());
        }

        [Fact]
        public async Task ChangeUserRole_ReturnsNotFound_WhenUserNotFound()
        {
            // Arrange
            int userId = 999;
            UserRole userRole = UserRole.Admin;
            _mockUserApplicationService.Setup(s => s.ChangeRoleAsync(userId, userRole))
                .ThrowsAsync(new KeyNotFoundException("User not found"));

            // Act
            var result = await _controller.ChangeUserRole(userId, userRole);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>(because: "Non-existent user should return 404 NotFound");
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull(because: "Result should contain a valid response");
            notFoundResult.StatusCode.Should().Be(404, because: "NotFoundResult should have status code 404");
            notFoundResult.Value.Should().NotBeNull(because: "NotFoundResult should contain a ProblemDetails object");
            notFoundResult.Value.As<ProblemDetails>().Detail.Should().Contain("User not found", because: "response should contain the exception message");
            _mockUserApplicationService.Verify(s => s.ChangeRoleAsync(userId, userRole), Times.Once());
        }

        [Fact]
        public async Task ChangeUserRole_ReturnsBadRequest_WhenUserIdIsInvalid()
        {
            // Arrange
            int userId = -1;
            UserRole userRole = UserRole.Admin;

            // Act
            var result = await _controller.ChangeUserRole(userId, userRole);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>(because: "Invalid user ID should return 400 BadRequest");
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull(because: "Result should contain a valid response");
            badRequestResult.StatusCode.Should().Be(400, because: "BadRequestResult should have status code 400");
            badRequestResult.Value.Should().NotBeNull(because: "BadRequestResult should contain a ProblemDetails object");
            badRequestResult.Value.As<ProblemDetails>().Detail.Should().Contain("Invalid user ID", because: "response should indicate the invalid user ID");
            _mockUserApplicationService.Verify(s => s.ChangeRoleAsync(It.IsAny<int>(), It.IsAny<UserRole>()), Times.Never());
        }

        [Fact]
        public async Task ChangeUserRole_ReturnsBadRequest_WhenUserRoleIsInvalid()
        {
            // Arrange
            int userId = 1;
            UserRole userRole = (UserRole)(-1); 
            _mockUserApplicationService.Setup(s => s.ChangeRoleAsync(userId, userRole))
                .ThrowsAsync(new ArgumentException("Invalid user role specified"));

            // Act
            var result = await _controller.ChangeUserRole(userId, userRole);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>(because: "Invalid user role should return 400 BadRequest");
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull(because: "Result should contain a valid response");
            badRequestResult.StatusCode.Should().Be(400, because: "BadRequestResult should have status code 400");
            badRequestResult.Value.Should().NotBeNull(because: "BadRequestResult should contain a ProblemDetails object");
            badRequestResult.Value.As<ProblemDetails>().Detail.Should().Contain("Invalid user role specified", because: "response should contain the exception message");
            _mockUserApplicationService.Verify(s => s.ChangeRoleAsync(userId, userRole), Times.Once());
        }

        [Fact]
        public async Task ChangeRole_ReturnsInternalServerError_WhenUnexpectedExceptionOccurs()
        {
            // Arrange
            int userId = 1;
            UserRole userRole = UserRole.Admin;
            _mockUserApplicationService.Setup(s => s.ChangeRoleAsync(userId, userRole))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.ChangeUserRole(userId, userRole);

            // Assert
            result.Should().BeOfType<ObjectResult>(because: "Unexpected errors should return 500 InternalServerError");
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull(because: "Result should contain a valid response");
            objectResult.StatusCode.Should().Be(500, because: "ObjectResult should have status code 500");
            objectResult.Value.Should().NotBeNull(because: "ObjectResult should contain a ProblemDetails object");
            objectResult.Value.As<ProblemDetails>().Detail.Should().Contain("Unexpected error occured", because: "response should contain the exception message");
            _mockUserApplicationService.Verify(s => s.ChangeRoleAsync(userId, userRole), Times.Once());
        }

    }
}
