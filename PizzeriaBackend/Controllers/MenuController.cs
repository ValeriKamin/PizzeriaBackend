using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzeriaBackend.Data.Interfaces;
using PizzeriaBackend.Models.Orders;

namespace PizzeriaBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly IFoodRepository _repo;

        public MenuController(IFoodRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("all")]
        public IActionResult GetAllFoods()
        {
            var foods = _repo.GetAllFoods();
            return Ok(foods);
        }


        [HttpPut("update-price")]
        //[Authorize(Roles = "Admin")]
        public IActionResult UpdatePrice([FromBody] UpdatePriceModel model)
        {
            var foods = _repo.GetAllFoods();
            var food = foods.FirstOrDefault(f => f.Id == model.FoodId);

            if (food == null)
                return NotFound(new { message = "Продукт не знайдено" });

            _repo.UpdatePrice(model.FoodId, model.NewPrice);

            return Ok(new { message = "Ціну оновлено" });
        }
    }


}
