using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(UserRegisterDTO registerDTO)
        {
            var user = new User
            {
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                Email = registerDTO.Email,
                Role = "Customer"
            };
            var registeredUser = await _userRepository.RegisterUser(user, registerDTO.Password);

            return Ok(new UserDTO
            {
                Id = registeredUser.Id,
                FirstName = registeredUser.FirstName,
                LastName = registeredUser.LastName,
                Email = registeredUser.Email,
                Role = registeredUser.Role
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserLoginDTO loginDTO)
        {
            var user = await _userRepository.LoginUser(loginDTO.Email, loginDTO.Password);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var secretKey = _configuration["Jwt:SecretKey"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var token = JwtHelper.GenerateToken(user, secretKey!, issuer!, audience!);

            return Ok(new
            {
                Token = token,
                User = new UserDTO
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role
                }
            });
        }
    }
}
