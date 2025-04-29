using NUnit.Framework;
using Moq;
using Pizzeria.Controllers;
using PizzeriaBackend.Data;
using Pizzeria.Models;
using Pizzeria.Helpers;
using PizzeriaBackend.Services;
using Microsoft.AspNetCore.Mvc;
using static PizzeriaBackend.Services.JwtService;

namespace Pizzeria.Tests
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IUserRepository> _userRepoMock;
        private Mock<JwtService> _jwtServiceMock;
        private AuthController _controller;

        [SetUp]
        public void Setup()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _jwtServiceMock = new Mock<JwtService>(null);

            _controller = new AuthController(_userRepoMock.Object, _jwtServiceMock.Object);
        }

        [Test]
        public void Login_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var testUser = new User
            {
                Username = "admin",
                PasswordHash = PasswordHelper.ComputeSha256Hash("123456"),
                Role = "Admin"
            };

            var userRepoMock = new Mock<IUserRepository>();
            var jwtServiceMock = new Mock<IJwtService>();

            userRepoMock.Setup(r => r.GetByUsername("admin")).Returns(testUser);
            jwtServiceMock.Setup(j => j.GenerateJwtToken(testUser)).Returns("test.jwt.token");

            var controller = new AuthController(userRepoMock.Object, jwtServiceMock.Object);

            var loginModel = new LoginModel
            {
                Username = "admin",
                Password = "123456"
            };

            var result = controller.Login(loginModel);

            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var response = okResult.Value as LoginResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Token, Is.EqualTo("test.jwt.token"));
        }

        [Test]
        public void Login_WithWrongPassword_ReturnsUnauthorized()
        {
            // Arrange
            var testUser = new User
            {
                Username = "admin",
                PasswordHash = PasswordHelper.ComputeSha256Hash("correctpass"),
                Role = "Admin"
            };

            _userRepoMock.Setup(r => r.GetByUsername("admin")).Returns(testUser);

            var loginModel = new LoginModel
            {
                Username = "admin",
                Password = "wrongpass"
            };

            // Act
            var result = _controller.Login(loginModel);

            // Assert
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
        }
    }
}
