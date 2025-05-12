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
using System.Text.Json;

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

        [Test]
        public void GetAllReviews_ReturnsListOfReviews()
        {
            // Arrange
            var expectedReviews = new List<Review>
            {
             new Review { ReviewId = 1, Name = "Іван", Topic = "Сервіс", Comment = "Все чудово!", PhoneNumber = "380111111111", CreatedAt = DateTime.Now },
            new Review { ReviewId = 2, Name = "Оля", Topic = "Їжа", Comment = "Смачно!", PhoneNumber = "380222222222", CreatedAt = DateTime.Now }
            };

            var reviewRepoMock = new Mock<IReviewRepository>();
            reviewRepoMock.Setup(r => r.GetAllReviews()).Returns(expectedReviews);

            var controller = new ReviewsController(reviewRepoMock.Object);

            // Act
            var result = controller.GetAllReviews();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okResult = result as OkObjectResult;
            Assert.That(okResult!.Value, Is.InstanceOf<List<Review>>());

            var actualReviews = okResult.Value as List<Review>;
            Assert.That(actualReviews!.Count, Is.EqualTo(2));
            Assert.That(actualReviews[0].Name, Is.EqualTo("Іван"));

            reviewRepoMock.Verify(r => r.GetAllReviews(), Times.Once);
        }
    }

    [TestFixture]
    public class MenuControllerTests
    {
        [Test]
        public void GetAllFoods_ReturnsListOfFoods()
        {
            // Arrange
            var foodList = new List<PizzeriaBackend.Models.Food>
            {
                new PizzeriaBackend.Models.Food { Id = 1, Name = "Маргарита", Quantity = "Класична", Weight = 450, Price = 129.99M },
                new PizzeriaBackend.Models.Food { Id = 2, Name = "Пепероні", Quantity = "Гостра", Weight = 500, Price = 149.99M }
            };

            var foodRepoMock = new Mock<IFoodRepository>();
            foodRepoMock.Setup(r => r.GetAllFoods()).Returns(foodList);

            var controller = new MenuController(foodRepoMock.Object);

            // Act
            var result = controller.GetAllFoods();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okResult = result as OkObjectResult;
            Assert.That(okResult!.Value, Is.InstanceOf<List<PizzeriaBackend.Models.Food>>());

            var actualList = okResult.Value as List<PizzeriaBackend.Models.Food>;
            Assert.That(actualList!.Count, Is.EqualTo(2));
            Assert.That(actualList[0].Name, Is.EqualTo("Маргарита"));

            foodRepoMock.Verify(r => r.GetAllFoods(), Times.Once);
        }

        [Test]
        public void AddToCart_ValidProduct_AddsItemAndReturnsOk()
        {
            // Arrange
            var testFood = new PizzeriaBackend.Models.Food
            {
                Id = 1,
                Name = "Маргарита",
                Quantity = "Класична",
                Weight = 450,
                Price = 129.99m
            };

            var foodRepoMock = new Mock<IFoodRepository>();
            foodRepoMock.Setup(r => r.GetAllFoods()).Returns(new List<PizzeriaBackend.Models.Food> { testFood });

            var cartRepoMock = new Mock<ICartRepository>();
            cartRepoMock.Setup(r => r.AddItem(It.IsAny<CartItem>()));

            var controller = new CartController(cartRepoMock.Object, foodRepoMock.Object);

            var model = new AddToCartModel
            {
                FoodId = 1,
                Quantity = 2,
                Username = "test_user"
            };

            // Act
            var result = controller.AddToCart(model);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var response = okResult.Value?.ToString();
            Assert.That(response, Does.Contain("Додано до кошика"));

            cartRepoMock.Verify(r => r.AddItem(It.IsAny<CartItem>()), Times.Once);
        }

        [Test]
        public void UpdatePrice_ValidFoodId_UpdatesAndReturnsOk()
        {
            // Arrange
            var foodList = new List<PizzeriaBackend.Models.Food>
            {
            new PizzeriaBackend.Models.Food { Id = 1, Name = "Маргарита", Price = 129.99m, Weight = 450, Quantity = "Класика" }
            };

            var foodRepoMock = new Mock<IFoodRepository>();
            foodRepoMock.Setup(r => r.GetAllFoods()).Returns(foodList);
            foodRepoMock.Setup(r => r.UpdatePrice(1, 179.99m));

            var controller = new MenuController(foodRepoMock.Object);

            var model = new UpdatePriceModel
            {
                FoodId = 1,
                NewPrice = 179.99m
            };

            // Act
            var result = controller.UpdatePrice(model);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var response = okResult!.Value?.ToString();
            Assert.That(response, Does.Contain("Ціну оновлено"));

            foodRepoMock.Verify(r => r.UpdatePrice(1, 179.99m), Times.Once);
        }
    }

    [TestFixture]
    public class CartControllerTests
    {
        [Test]
        public void GetCart_ReturnsItemsAndTotalPrice()
        {
            // Arrange
            var username = "test_user";

            var cartItems = new List<CartItem>
            {
                new CartItem
                {
                    FoodName = "Маргарита",
                    Quantity = 2,
                    Weight = 500,
                    Price = 129.99m * 2,
                    Username = username
                },
                new CartItem
                {
                    FoodName = "Пепероні",
                    Quantity = 1,
                    Weight = 450,
                    Price = 149.99m,
                    Username = username
                }
            };

            var cartRepoMock = new Mock<ICartRepository>();
            cartRepoMock.Setup(r => r.GetItemsByUser(username)).Returns(cartItems);

            var foodRepoMock = new Mock<IFoodRepository>(); // для конструктора

            var controller = new CartController(cartRepoMock.Object, foodRepoMock.Object);

            // Act
            var result = controller.GetCart(username);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var json = JsonSerializer.Serialize(okResult!.Value, new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            Assert.That(json, Does.Contain("Маргарита"));
            Assert.That(json, Does.Contain("Пепероні"));
            Assert.That(json, Does.Contain("409.97")); // 2x129.99 + 149.99

            cartRepoMock.Verify(r => r.GetItemsByUser(username), Times.Once);
        }

        [Test]
        public void CreateOrder_ValidCart_CreatesOrderAndReturnsOk()
        {
            // Arrange
            var username = "test_user";

            var cartItems = new List<CartItem>
            {
                new CartItem { FoodName = "Пепероні", Quantity = 2, Weight = 500, Price = 259.99m, Username = username },
                new CartItem { FoodName = "4 сира", Quantity = 3, Weight = 680, Price = 659.00m, Username = username }
            };

            var cartRepoMock = new Mock<ICartRepository>();
            cartRepoMock.Setup(r => r.GetItemsByUser(username)).Returns(cartItems);
            cartRepoMock.Setup(r => r.ClearCart(username));

            var orderRepoMock = new Mock<IOrderRepository>();
            orderRepoMock.Setup(r => r.CreateOrder(It.IsAny<Order>()));

            var controller = new OrdersController(orderRepoMock.Object, cartRepoMock.Object);

            var model = new CreateOrderModel
            {
                Username = username,
                //FullName = "Іван",
                Phone = "+380123456789",
                Email = "test@example.com",
                DeliveryType = "Доставка",
                Address = "вул. Прикладна, 1",
                Apartment = "12",
                Entrance = "2",
                Floor = "3",
                DoorCode = "1234",
                CourierComment = "Подзвоніть",
                DeliveryTime = "18:00",
                CardNumber = "1234567812345678",
                CVM = "123",
                Expiry = "12/25"
            };

            // Act
            var result = controller.Create(model);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var response = okResult!.Value!.ToString();
            Assert.That(response, Does.Contain("Замовлення оформлено"));

            cartRepoMock.Verify(r => r.GetItemsByUser(username), Times.Once);
            cartRepoMock.Verify(r => r.ClearCart(username), Times.Once);
            orderRepoMock.Verify(r => r.CreateOrder(It.IsAny<Order>()), Times.Once);
        }
    }

    [TestFixture]
    public class OrdersControllerTests
    {
        [Test]
        public void GetUserOrders_ReturnsOrdersList()
        {
            // Arrange
            var username = "test_user";
            var orders = new List<Order>
            {
                new Order
                {
                    Id = 1,
                    Username = username,
                    CreatedAt = new DateTime(2025, 4, 2),
                    CardNumber = "1234123412341234",
                    Total = 1900m,
                    Status = "Обробляється"
                },
                new Order
                {
                    Id = 2,
                    Username = username,
                    CreatedAt = new DateTime(2025, 4, 1),
                    CardNumber = "Готівка",
                    Total = 1900m,
                    Status = "Завершено"
                }
            };

            var orderRepoMock = new Mock<IOrderRepository>();
            orderRepoMock.Setup(r => r.GetOrdersByUser(username)).Returns(orders);

            var cartRepoMock = new Mock<ICartRepository>(); 
            var controller = new OrdersController(orderRepoMock.Object, cartRepoMock.Object);

            // Act
            var result = controller.GetUserOrders(username);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var json = JsonSerializer.Serialize(okResult!.Value,
                new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

            Assert.That(json, Does.Contain("1900"));
            Assert.That(json, Does.Contain("Обробляється"));
            Assert.That(json, Does.Contain("Оплата карткою"));
            Assert.That(json, Does.Contain("Оплата готівкою"));

            orderRepoMock.Verify(r => r.GetOrdersByUser(username), Times.Once);
        }
    }

    [TestFixture]
    public class OrdersControllerTestsAdmin
    {
        [Test]
        public void GetAllByStatus_ReturnsFilteredOrdersForAdmin()
        {
            // Arrange
            var status = "Завершено";

            var orders = new List<Order>
            {
                new Order
                {
                    Id = 1001,
                    Username = "Клієнт 1",
                    Phone = "+380000000001",
                    Address = "вул. А",
                    CardNumber = "1234567812345678",
                    Total = 1800m,
                    Status = status,
                    CreatedAt = new DateTime(2025, 4, 2)
                },
                new Order
                {
                    Id = 1002,
                    Username = "Клієнт 2",
                    Phone = "+380000000002",
                    Address = "вул. Б",
                    CardNumber = "Готівка",
                    Total = 1000m,
                    Status = status,
                    CreatedAt = new DateTime(2025, 4, 2)
                }
            };

            var orderRepoMock = new Mock<IOrderRepository>();
            orderRepoMock.Setup(r => r.GetOrdersByStatus(status)).Returns(orders);

            var cartRepoMock = new Mock<ICartRepository>(); // для конструктора
            var controller = new OrdersController(orderRepoMock.Object, cartRepoMock.Object);

            // Act
            var result = controller.GetAllByStatus(status);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var json = JsonSerializer.Serialize(okResult!.Value,
                new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

            Assert.That(json, Does.Contain("Клієнт 1"));
            Assert.That(json, Does.Contain("Оплата карткою"));
            Assert.That(json, Does.Contain("Оплата готівкою"));
            Assert.That(json, Does.Contain("1800"));
            Assert.That(json, Does.Contain("1000"));
            Assert.That(json, Does.Contain("02.04.2025"));

            orderRepoMock.Verify(r => r.GetOrdersByStatus(status), Times.Once);
        }

        [Test]
        public void UpdateStatus_ValidCall_UpdatesAndReturnsOk()
        {
            // Arrange
            var orderId = 1234;
            var newStatus = "Завершено";

            var orderRepoMock = new Mock<IOrderRepository>();
            orderRepoMock.Setup(r => r.UpdateStatus(orderId, newStatus));

            var cartRepoMock = new Mock<ICartRepository>(); // для конструктора

            var controller = new OrdersController(orderRepoMock.Object, cartRepoMock.Object);

            var model = new UpdateStatusModel
            {
                OrderId = orderId,
                NewStatus = newStatus
            };

            // Act
            var result = controller.UpdateStatus(model);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());

            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);

            var json = JsonSerializer.Serialize(okResult!.Value,
                new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

            Assert.That(json, Does.Contain("Статус змінено"));

            orderRepoMock.Verify(r => r.UpdateStatus(orderId, newStatus), Times.Once);
        }
    }
}
