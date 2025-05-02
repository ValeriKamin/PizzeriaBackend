using Microsoft.AspNetCore.Mvc;
using PizzeriaBackend.Data;

namespace PizzeriaBackend.Models
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

        [HttpPost("create")]
        public IActionResult Create([FromBody] CreateOrderModel model)
        {
            var cartItems = _cartRepo.GetItemsByUser(model.Username);
            if (!cartItems.Any())
                return BadRequest(new { message = "Кошик порожній" });

            var total = cartItems.Sum(i => i.Price);

            var order = new Order
            {
                Username = model.Username,
                FullName = model.FullName,
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
                CardNumber = model.CardNumber,
                CVM = model.CVM,
                Expiry = model.Expiry,
                Total = total,
                Status = "Обробляється",
                CreatedAt = DateTime.Now
            };

            _orderRepo.CreateOrder(order);
            _cartRepo.ClearCart(model.Username);

            return Ok(new { message = "Замовлення оформлено!" });
        }
    }
}
