using NUnit.Framework;
using Moq;
using Pizzeria;
using Pizzeria.Controllers;
using Pizzeria.Data;
using Pizzeria.Models;
using Pizzeria.Helpers;
using Microsoft.AspNetCore.Mvc;
using PizzeriaBackend.Data;

namespace Pizzeria.Tests
{
    [TestFixture]
    public class AuthControllerTests
    {
        [Test]
        public void Login_WithValidCredentials_ReturnsOk()
        {
            // Arrange
            var user = new User
            {
                Username = "admin",
                PasswordHash = PasswordHelper.ComputeSha256Hash("123456"),
                Role = "Admin"
            };

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetByUsername("admin")).Returns(user);

            var controller = new AuthController(mockRepo.Object);
            var model = new LoginModel { Username = "admin", Password = "123456" };

            // Act
            var result = controller.Login(model);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var response = result as OkObjectResult;
            Assert.That(((dynamic)response.Value).username, Is.EqualTo("admin"));
            Assert.That(((dynamic)response.Value).role, Is.EqualTo("Admin"));
        }

        [Test]
        public void Login_WithWrongPassword_ReturnsUnauthorized()
        {
            // Arrange
            var user = new User
            {
                Username = "admin",
                PasswordHash = PasswordHelper.ComputeSha256Hash("correct_password"),
                Role = "Admin"
            };

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetByUsername("admin")).Returns(user);

            var controller = new AuthController(mockRepo.Object);
            var model = new LoginModel { Username = "admin", Password = "wrong_password" };

            // Act
            var result = controller.Login(model);

            // Assert
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
        }
    }
}
