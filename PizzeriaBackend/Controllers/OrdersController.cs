using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using PizzeriaBackend.Data;
using PizzeriaBackend.Models;
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
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllByStatus([FromQuery] string status)
        {
            var orders = _orderRepo.GetOrdersByStatus(status);

            var result = orders.Select(o => new
            {
                o.Id,
                o.FullName,
                o.Phone,
                o.Address,
                o.Total,
                //PaymentMethod = o.CardNumber == "Готівка" ? "Оплата готівкою" : "Оплата карткою",
                Date = o.CreatedAt.ToString("dd.MM.yyyy"),
                o.Status,
                ItemsCount = 9 // тимчасово — або доопрацюємо
            });

            return Ok(result);
        }

        //[HttpPost("create")]
        //public IActionResult Create([FromBody] CreateOrderModel model)
        //{
        //    //var cartItems = _cartRepo.GetItemsByUser(model.Username);
        //    //if (!cartItems.Any())
        //    //    return BadRequest(new { message = "Кошик порожній" });

        //    if (model.Items == null || !model.Items.Any())
        //        return BadRequest(new { message = "Кошик порожній" });

        //    //var total = cartItems.Sum(i => i.Price);
        //    var total = model.Items.Sum(i => i.Price * i.Quantity);

        //    var order = new Order
        //    {
        //        Username = model.Username,
        //        FullName = model.FullName,
        //        Phone = model.Phone,
        //        Email = model.Email,
        //        DeliveryType = model.DeliveryType,
        //        Address = model.Address,
        //        Apartment = model.Apartment,
        //        Entrance = model.Entrance,
        //        Floor = model.Floor,
        //        DoorCode = model.DoorCode,
        //        CourierComment = model.CourierComment,
        //        //DeliveryTime = model.DeliveryTime,
        //        //CardNumber = model.CardNumber,
        //        //CVM = model.CVM,
        //        Expiry = model.Expiry,
        //        Total = total,
        //        Status = "Обробляється",
        //        CreatedAt = DateTime.Now
        //    };

        //    _orderRepo.CreateOrder(order);
        //    //_cartRepo.ClearCart(model.Username);

        //    return Ok(new { message = "Замовлення оформлено!" });
        //}

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
                //FullName = model.FullName,
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
                //CardNumber = model.CardNumber,
                //CVM = model.CVM,
                //Expiry = model.Expiry,
                Total = total,
                Status = "Обробляється",
                CreatedAt = DateTime.Now
            };

            _orderRepo.CreateOrder(order);

            if (model.Items == null && !string.IsNullOrWhiteSpace(model.Username))
            {
                _cartRepo.ClearCart(model.Username);
            }

            return Ok(new { message = "Замовлення оформлено!" });
        }

        [HttpGet("user/{username}")]
        public IActionResult GetUserOrders(string username)
        {
            var orders = _orderRepo.GetOrdersByUser(username);

            var result = orders.Select(o => new
            {
                o.Id,
                o.Status,
                PaymentMethod = o.CardNumber == "Готівка" ? "Оплата готівкою" : "Оплата карткою",
                Date = o.CreatedAt.ToString("dd.MM.yyyy"),
                o.Total,
                ItemsCount = 9 // тимчасово — можна буде вирахувати або зберігати
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

        [HttpGet("user/{username}")]
        public IActionResult GetByUser(string username)
        {
            var orders = _orderRepo.GetOrdersByUsername(username);
            return Ok(orders);
        }
    }

}
