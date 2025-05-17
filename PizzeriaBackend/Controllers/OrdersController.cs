using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using PizzeriaBackend.Data.Interfaces;
using PizzeriaBackend.Models.Cart;
using PizzeriaBackend.Models.Orders;
using System.Text.Json;


namespace PizzeriaBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {

        private readonly IOrderRepository _orderRepo;
        private readonly ICartRepository _cartRepo;

        public OrdersController(IOrderRepository orderRepo, ICartRepository cartRepo)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
        }

        [HttpGet("admin")]
        //[Authorize(Roles = "Admin")]
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
                o.Items
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
                cartItems = _cartRepo.GetItemsByUser(model.Username);
                if (!cartItems.Any())
                    return BadRequest(new { message = "Кошик порожній" });
            }

            var total = cartItems.Sum(i => i.Price * i.Quantity);

            var order = new Order
            {
                Username = model.Username,
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
                o.Status,
                o.Total,
                o.CreatedAt,
                o.Items
            });

            return Ok(result);
        }

        [HttpPut("update-status")]
        //[Authorize(Roles = "Admin")]
        public IActionResult UpdateStatus([FromBody] UpdateStatusModel model)
        {
            _orderRepo.UpdateStatus(model.OrderId, model.NewStatus);
            return Ok(new { message = "Статус змінено" });
        }

    }

}
