using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskService.Application;
using TaskService.Application.DTOs;
using TaskService.Presentation.Controllers;

namespace TaskService.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IUserApplicationService> _mockUserApplicationService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockUserApplicationService = new Mock<IUserApplicationService>();
            _controller = new AuthController(_mockUserApplicationService.Object);
        }

        [Fact]
        public async Task Register_ReturnsOkWithToken_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var registerDto = new RegisterUserDto("test@example.com", "Pass123!");
            var token = "jwt-token-123";
            _mockUserApplicationService.Setup(s => s.RegisterAsync(registerDto)).ReturnsAsync(token);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>(because: "Valid DTO should return 200 OK");
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull(because: "Result should contain a valid response");
            okResult.StatusCode.Should().Be(200, because: "Status code should be 200 OK");
            okResult.Value.Should().NotBeNull(because: "Response value should not be null");
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(okResult.Value);
            var response = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            response.Should().ContainKey("Token", because: "Response should contain 'Token' property");
            response["Token"].Should().Be(token, because: "Token should match service output exactly");
            _mockUserApplicationService.Verify(s => s.RegisterAsync(It.IsAny<RegisterUserDto>()), Times.Once());
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenRegistrationFailsWithInvalidOperationException()
        {
            // Arrange
            var registerDto = new RegisterUserDto("duplicate@example.com", "Pass123!");
            _mockUserApplicationService.Setup(s => s.RegisterAsync(It.IsAny<RegisterUserDto>()))
                .ThrowsAsync(new InvalidOperationException("Email already exists"));

            // Act
            var result = await _controller.Register(registerDto);

            // Debug
            Console.WriteLine($"Result Type: {result.GetType().Name}");
            if (result is ObjectResult objResult)
                Console.WriteLine($"Status: {objResult.StatusCode}, Value: {Newtonsoft.Json.JsonConvert.SerializeObject(objResult.Value)}");

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>(because: "Duplicate email should return 400 BadRequest");
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull(because: "Result should contain a valid response");
            badRequestResult.StatusCode.Should().Be(400);
            badRequestResult.Value.As<ProblemDetails>().Detail.Should().Contain("Email already exists");
        }

        // Fix for CS8602: Dereference of a possibly null reference.
        // Add null checks before dereferencing errorResult in Register_ReturnsInternalServerError_WhenUnexpectedExceptionOccurs and Login_ReturnsInternalServerError_WhenUnexpectedExceptionOccurs

        [Fact]
        public async Task Register_ReturnsInternalServerError_WhenUnexpectedExceptionOccurs()
        {
            // Arrange
            var registerDto = new RegisterUserDto("test@example.com", "Pass123!");
            _mockUserApplicationService.Setup(s => s.RegisterAsync(registerDto))
                .ThrowsAsync(new Exception("Unexpected error, e.g., database failure"));

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            result.Should().BeOfType<ObjectResult>(because: "Unexpected exception should return 500 InternalServerError");
            var errorResult = result as ObjectResult;
            errorResult.Should().NotBeNull(because: "Result should contain a valid response");
            errorResult!.StatusCode.Should().Be(500);
            errorResult.Value.As<ProblemDetails>().Detail.Should().Contain("Unexpected error");
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = await _controller.Register(null!);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>(because: "Null DTO should return 400 BadRequest");
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull(because: "Result should contain a valid response");
            badRequestResult.StatusCode.Should().Be(400, because: "Status code should be 400 BadRequest");
            badRequestResult.Value.Should().NotBeNull(because: "Response value should not be null");
            badRequestResult.Value.As<ProblemDetails>().Detail.Should().Contain("Request body is null", because: "Response should indicate null DTO error");
            _mockUserApplicationService.Verify(s => s.RegisterAsync(It.IsAny<RegisterUserDto>()), Times.Never());
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenModelIsInvalid()
        {
            // Arrange
            var registerDto = new RegisterUserDto("", "Pass123!");
            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>(because: "Invalid model state should return 400 BadRequest");
            _mockUserApplicationService.Verify(s => s.RegisterAsync(It.IsAny<RegisterUserDto>()), Times.Never());
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenEmailIsMalformed()
        {
            // Arrange
            var registerDto = new RegisterUserDto("invalid-email", "Pass123!");
            _controller.ModelState.AddModelError("Email", "Invalid email format");

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>(because: "Malformed email should return 400 BadRequest");
            _mockUserApplicationService.Verify(s => s.RegisterAsync(It.IsAny<RegisterUserDto>()), Times.Never());
        }

        [Fact]
        public async Task Login_ReturnsOkWithToken_WhenLoginSucceeds()
        {
            // Arrange
            var loginDto = new LoginUserDto("test@example.com", "Pass123!");
            var token = "jwt-token-456";
            _mockUserApplicationService.Setup(s => s.LoginAsync(loginDto)).ReturnsAsync(token);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>(because: "Valid login should return 200 OK");
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull(because: "Result should contain a valid response");
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().NotBeNull(because: "Response value should not be null");
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(okResult.Value);
            var response = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            response.Should().ContainKey("Token", because: "Response should contain 'Token' property");
            response["Token"].Should().Be(token, because: "Token should match service output");
            _mockUserApplicationService.Verify(s => s.LoginAsync(loginDto), Times.Once());
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = await _controller.Login(null!);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>(because: "Null DTO should return 400 BadRequest");
            _mockUserApplicationService.Verify(s => s.LoginAsync(It.IsAny<LoginUserDto>()), Times.Never());
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenModelIsInvalid()
        {
            // Arrange
            var loginDto = new LoginUserDto("", "Pass123!");
            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>(because: "Invalid model state should return 400 BadRequest");
            _mockUserApplicationService.Verify(s => s.LoginAsync(It.IsAny<LoginUserDto>()), Times.Never());
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenEmailIsMalformed()
        {
            // Arrange
            var loginDto = new LoginUserDto("invalid-email", "Pass123!");
            _controller.ModelState.AddModelError("Email", "Invalid email format");

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>(because: "Malformed email should return 400 BadRequest");
            _mockUserApplicationService.Verify(s => s.LoginAsync(It.IsAny<LoginUserDto>()), Times.Never());
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginDto = new LoginUserDto("wrong@example.com", "wrong");
            _mockUserApplicationService.Setup(s => s.LoginAsync(loginDto))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid email or password"));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            result.Should().BeOfType<UnauthorizedObjectResult>(because: "Invalid credentials should return 401 Unauthorized");
            var unauthorizedResult = result as UnauthorizedObjectResult;
            unauthorizedResult.Should().NotBeNull(because: "Result should contain a valid response");
            unauthorizedResult.StatusCode.Should().Be(401);
            unauthorizedResult.Value.As<ProblemDetails>().Detail.Should().Contain("Invalid email or password");
        }

        [Fact]
        public async Task Login_ReturnsInternalServerError_WhenUnexpectedExceptionOccurs()
        {
            // Arrange
            var loginDto = new LoginUserDto("test@example.com", "Pass123!");
            _mockUserApplicationService.Setup(s => s.LoginAsync(loginDto))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            result.Should().BeOfType<ObjectResult>(because: "Unexpected exception should return 500 InternalServerError");
            var errorResult = result as ObjectResult;
            errorResult.Should().NotBeNull(because: "Result should contain a valid response");
            errorResult!.StatusCode.Should().Be(500);
            errorResult.Value.As<ProblemDetails>().Detail.Should().Contain("Unexpected error");
        }
    }
}
