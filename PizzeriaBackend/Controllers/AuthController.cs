using Microsoft.AspNetCore.Mvc;
using Pizzeria.Data;
using Pizzeria.Models;
using Pizzeria.Data;
using System.Runtime.InteropServices;
using Pizzeria.Helpers;
using PizzeriaBackend.Services;
using static PizzeriaBackend.Services.JwtService;
using PizzeriaBackend.Models;
using PizzeriaBackend.Models.Auth;
using PizzeriaBackend.Data.Interfaces;

namespace Pizzeria.Controllers
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
                return Unauthorized("Користувача не знайдено");

            var hash = PasswordHelper.ComputeSha256Hash(model.Password);
            if (hash != user.PasswordHash)
                return Unauthorized("Невірний пароль");

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
            var existingUser = _userRepo.GetByUsername(model.Username);
            if (existingUser != null)
            {
                return Conflict(new { message = "Користувач з таким ім’ям вже існує" });
            }

            var newUser = new User
            {
                Username = model.Username,
                PasswordHash = PasswordHelper.ComputeSha256Hash(model.Password),
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Role = model.Role
            };

            _userRepo.CreateUser(newUser);

            return Ok(new RegisterResponse { Message = "Успішна реєстрація" });
        }

        //[HttpGet("test")]
        //public IActionResult Test()
        //{
        //    return Ok(new LoginResponse
        //    {
        //        Message = "Авторизація успішна"
        //    });
        //}
    }



}

