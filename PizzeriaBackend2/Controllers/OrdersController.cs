using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using PizzeriaBackend.Data.Interfaces;
using PizzeriaBackend.Models.Cart;
using PizzeriaBackend.Models.Orders;
using System.Text.Json;
using System.Text.RegularExpressions;


namespace PizzeriaBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {

        private readonly IOrderRepository _orderRepo;
        private readonly ICartRepository _cartRepo;

        private readonly IUserRepository _userRepo;

        public OrdersController(IOrderRepository orderRepo, ICartRepository cartRepo, IUserRepository userRepo)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
            _userRepo = userRepo;
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllByStatus([FromQuery] string status)
        {
            var orders = _orderRepo.GetOrdersByStatus(status);

            var result = orders.Select(o => new {
                o.Id,
                o.Username,
                o.Phone,
                o.Email,
                o.DeliveryType,
                o.Address,
                o.Apartment,
                o.Entrance,
                o.Floor,
                o.DoorCode,
                o.CourierComment,
                o.Total,
                o.Status,
                o.DeliveryTime,
                o.CreatedAt,
            });

            return Ok(result);
        }

        [HttpGet("admin/full")]
        public IActionResult GetAllByStatusWithItems([FromQuery] string status)
        {
            var orders = _orderRepo.GetOrdersWithItemsByStatus(status); 

            var result = orders.Select(o => new {
                o.Id,
                o.Username,
                o.Phone,
                o.Email,
                o.DeliveryType,
                o.Address,
                o.Apartment,
                o.Entrance,
                o.Floor,
                o.DoorCode,
                o.CourierComment,
                o.Total,
                o.Status,
                o.CreatedAt,
                o.DeliveryTime,
                o.Items,
                o.Name
            });

            return Ok(result);
        }


        [HttpPost("create")]
        public IActionResult Create([FromBody] CreateOrderModel model)
        {
            List<CartItem> cartItems;

            if (model.Items?.Any() == true)
            {
                cartItems = model.Items;
            }
            else
            {
                cartItems = _cartRepo.GetItemsByUser(model.ProfileName);
                if (!cartItems.Any())
                    return BadRequest(new { message = "Кошик порожній" });
            }

            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(model.Phone) || !Regex.IsMatch(model.Phone, @"^\+380\d{9}$"))
                errors.Add("Невірний номер телефону (формат +380...)");

            if (string.IsNullOrWhiteSpace(model.Email) || !Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                errors.Add("Невірний email");

            if (string.IsNullOrWhiteSpace(model.Address))
                errors.Add("Адреса не може бути порожньою");

            if (string.IsNullOrWhiteSpace(model.DeliveryTime))
                errors.Add("Час доставки обов’язковий");

            if (!string.IsNullOrWhiteSpace(model.CardNumber) && !Regex.IsMatch(model.CardNumber, @"^\d{16}$"))
                errors.Add("Невірний номер картки");

            if (!string.IsNullOrWhiteSpace(model.CVM) && !Regex.IsMatch(model.CVM, @"^\d{3}$"))
                errors.Add("Невірний CVM");

            if (!string.IsNullOrWhiteSpace(model.Expiry) && !Regex.IsMatch(model.Expiry, @"^\d{2}/\d{2,4}$"))
                errors.Add("Невірний термін дії картки");

            if (errors.Any())
                return BadRequest(new { message = "Помилки валідації", errors });

            var user = _userRepo.GetByUsername(model.Username);
            if (user == null)
                return BadRequest(new { message = "Користувач не знайдений" });

            var total = cartItems.Sum(i => i.Price * i.Quantity);


            var order = new Order
            {
                Username = model.ProfileName,
                Name = model.Username,
                Phone = model.Phone,
                Email = model.Email,
                DeliveryType = model.DeliveryType,
                Address = model.Address,
                Apartment = model.Apartment,
                Entrance = model.Entrance,
                Floor = model.Floor,
                DoorCode = model.DoorCode,
                CourierComment = model.CourierComment,
                DeliveryTime = model.DeliveryTime,
                Total = total,
                Status = "Обробляється",
                UserId = user.UserId,
                CreatedAt = DateTime.Now
            };

            int orderId = _orderRepo.CreateOrder(order); 
            _orderRepo.AddOrderItems(orderId, cartItems); 

            if (model.Items == null && !string.IsNullOrWhiteSpace(model.Username))
            {
                _cartRepo.ClearCart(model.Username);
            }


            return Ok(new { message = "Замовлення оформлено!" });
        }

        [HttpGet("user/{username}")]
        public IActionResult GetByUser(string username)
        {
            var orders = _orderRepo.GetOrdersByUser(username); 

            var result = orders.Select(o => new {
                o.Id,
                o.DeliveryTime,
                o.Status,
                o.Total,
                o.CreatedAt,
                o.Items
            });

            return Ok(result);
        }

        [HttpPut("update-status")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateStatus([FromBody] UpdateStatusModel model)
        {
            _orderRepo.UpdateStatus(model.OrderId, model.NewStatus);
            return Ok(new { message = "Статус змінено" });
        }

        [HttpGet("busy-times")]
        public IActionResult GetBusyDeliveryTimes()
        {
            var times = _orderRepo.GetBusyDeliveryTimes();
            return Ok(times); 
        }

    }

}
