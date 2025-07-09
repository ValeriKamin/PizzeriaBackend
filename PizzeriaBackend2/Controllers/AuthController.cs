using Microsoft.AspNetCore.Mvc;
using Pizzeria.Data;
using Pizzeria.Models;
using System.Runtime.InteropServices;
using Pizzeria.Helpers;
using PizzeriaBackend.Services;
using static PizzeriaBackend.Services.JwtService;
using PizzeriaBackend.Models;
using PizzeriaBackend.Models.Auth;
using PizzeriaBackend.Data.Interfaces;
using System.Text.RegularExpressions;

namespace PizzeriaBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IJwtService _jwtService;

        public AuthController(IUserRepository userRepo, IJwtService jwtService)
        {
            _userRepo = userRepo;
            _jwtService = jwtService;
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var user = _userRepo.GetByUsername(model.Username);
            if (user == null)
                return Unauthorized(new { message = "Користувача з таким ім'ям не існує" });

            var hash = PasswordHelper.ComputeSha256Hash(model.Password);
            if (hash != user.PasswordHash)
                return Unauthorized(new { message = "Невірний пароль" });

            var token = _jwtService.GenerateJwtToken(user);

            return Ok(new LoginResponse
            {
                Message = "Авторизація успішна",
                Token = token
            });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || model.Username.Length < 3)
                return BadRequest(new { message = "Ім’я користувача має містити щонайменше 3 символи" });

            if (model.Password.Length < 6)
                return BadRequest(new { message = "Пароль має бути щонайменше 6 символів" });

            if (!Regex.IsMatch(model.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return BadRequest(new { message = "Некоректний email" });

            if (!Regex.IsMatch(model.PhoneNumber, @"^\+380\d{9}$"))
                return BadRequest(new { message = "Номер телефону має бути у форматі +380xxxxxxxxx" });

            if (_userRepo.GetByUsername(model.Username) != null)
                return Conflict(new { message = "Користувач з таким ім’ям вже існує" });

            if (_userRepo.GetByEmail(model.Email) != null)
                return Conflict(new { message = "Email вже використовується" });

            var newUser = new User
            {
                Username = model.Username,
                PasswordHash = PasswordHelper.ComputeSha256Hash(model.Password),
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Role = model.Role ?? "User"
            };

            _userRepo.CreateUser(newUser);

            var token = _jwtService.GenerateJwtToken(newUser);

            return Ok(new
            {
                message = "Реєстрація успішна",
                token
            });
        }


    }
}

