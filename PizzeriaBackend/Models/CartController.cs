using Microsoft.AspNetCore.Mvc;
using PizzeriaBackend.Data;
using PizzeriaBackend.Models;

namespace PizzeriaBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _repo;
        private readonly IFoodRepository _foodRepo;

        public CartController(ICartRepository repo, IFoodRepository foodRepo)
        {
            _repo = repo;
            _foodRepo = foodRepo;
        }

        [HttpPost("add")]
        public IActionResult AddToCart([FromBody] AddToCartModel model)
        {
            var food = _foodRepo.GetAllFoods().FirstOrDefault(f => f.Id == model.FoodId);
            if (food == null)
                return NotFound(new { message = "Продукт не знайдено" });

            var cartItem = new CartItem
            {
                FoodId = food.Id,
                FoodName = food.Name,
                Quantity = model.Quantity,
                Weight = food.Weight,
                Price = food.Price * model.Quantity,
                Username = model.Username // можно изменить для токена
            };

            _repo.AddItem(cartItem);

            return Ok(new { message = "Додано до кошика" });
        }
    }
}
