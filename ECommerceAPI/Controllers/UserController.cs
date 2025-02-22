using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService, IConfiguration configuration) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        private readonly IConfiguration _configuration = configuration;

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(UserRegisterDTO registerDTO)
        {
            var result = await _userService.RegisterUser(registerDTO);

            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ErrorType.Conflict => Conflict(result.ErrorMessage),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred." })
                };
            }

            return Ok(result.Data);
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDTO>> Login(UserLoginDTO loginDTO)
        {
            var result = await _userService.LoginUser(loginDTO);

            if (!result.Success)
            {
                return result.ErrorType switch
                {
                    ErrorType.BadRequest => BadRequest(result.ErrorMessage),
                    _ => StatusCode(500, new { Message = "An unexpected error occurred." })
                };
            }

            var (token, user) = result.Data;

            return Ok(new
            {
                Token = token,
                User = user
            });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserDTO>>> GetUsers()
        {
            return await _userService.GetAllUsers();
        }
    }
}
