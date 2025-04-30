using NUnit.Framework;
using Moq;
using Pizzeria.Controllers;
using PizzeriaBackend.Data;
using Pizzeria.Models;
using Pizzeria.Helpers;
using PizzeriaBackend.Services;
using Microsoft.AspNetCore.Mvc;
using static PizzeriaBackend.Services.JwtService;
using PizzeriaBackend.Models;
using PizzeriaBackend.Controllers;

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
            Assert.That(okResult, Is.Not.Null);

            var response = okResult.Value as LoginResponse;
            Assert.That(response, Is.Not.Null);
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
            Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
        }

        [Test]
        public void Register_NewUser_ReturnsSuccess()
        {
            // Arrange
            var newUser = new RegisterModel
            {
                Username = "newuser",
                Password = "123456",
                Email = "newuser@example.com",
                PhoneNumber = "+380000000000",
                Role = "User"
            };

            _userRepoMock.Setup(r => r.GetByUsername("newuser")).Returns((User?)null); 
            _userRepoMock.Setup(r => r.CreateUser(It.IsAny<User>()));

            var controller = new AuthController(_userRepoMock.Object, _jwtServiceMock.Object);

            // Act
            var result = controller.Register(newUser);

            // Assert
            var okResult = result as OkObjectResult; 
            Assert.That(okResult, Is.Not.Null);

            var response = okResult.Value as RegisterResponse;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Is.EqualTo("Успішна реєстрація"));

            _userRepoMock.Verify(r => r.CreateUser(It.IsAny<User>()), Times.Once);
        }


        [Test]
        public void Register_ExistingUsername_ReturnsConflict()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Username = "admin",
                Password = "123456"
            };

            _userRepoMock.Setup(r => r.GetByUsername("admin")).Returns(new User());

            var controller = new AuthController(_userRepoMock.Object, _jwtServiceMock.Object);

            // Act
            var result = controller.Register(registerModel);

            // Assert
            Assert.That(result, Is.InstanceOf<ConflictObjectResult>());
        }

    }

    [TestFixture]
    public class ReviewsControllerTests
    {
        [Test]
        public void AddReview_ValidModel_CallsRepositoryAndReturnsOk()
        {
            // Arrange
            var reviewModel = new ReviewModel
            {
                Name = "Іван",
                Topic = "Сервіс",
                Comment = "Все чудово!",
                PhoneNumber = "+380111111111"
            };

            var reviewRepoMock = new Mock<IReviewRepository>();
            var controller = new ReviewsController(reviewRepoMock.Object);

            // Act
            var result = controller.AddReview(reviewModel);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okResult = (OkObjectResult)result;
            Assert.That(okResult.Value, Is.Not.Null);

            var response = okResult.Value as ReviewResponse;
            Assert.That(response, Is.Not.Null);
            Assert.That(response!.Message, Is.EqualTo("Відгук додано!"));

            reviewRepoMock.Verify(r => r.AddReview(It.IsAny<Review>()), Times.Once);
        }
    }
}
