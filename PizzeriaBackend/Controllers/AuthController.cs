using Microsoft.AspNetCore.Mvc;
using Pizzeria.Data;
using Pizzeria.Models;
using Pizzeria.Data;
using Pizzeria.Models;
using System.Runtime.InteropServices;
using Pizzeria.Helpres;

namespace Pizzeria.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _userRepo = new UserRepository();

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var user = _userRepo.GetByUsername(model.Username);
            if (user == null)
                return Unauthorized("Користувача не знайдено");

            var hash = PasswordHelper.ComputeSha256Hash(model.Password);
            if (hash != user.PasswordHash)
                return Unauthorized("Невірний пароль");

            return Ok(new
            {
                message = "Авторизація успішна",
                user.Username,
                user.Role
            });
        }
    }
}
