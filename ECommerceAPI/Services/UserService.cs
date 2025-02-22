using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;

namespace ECommerceAPI.Services
{
    public class UserService(IUserRepository userRepository, IConfiguration configuration) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IConfiguration _configuration = configuration;

        public async Task<Result<UserDTO>> RegisterUser(UserRegisterDTO registerDTO)
        {
            var userExists = await _userRepository.GetUserByEmail(registerDTO.Email);

            if (userExists != null)
            {
                return Result<UserDTO>.Failure("User with this email already exists", ErrorType.Conflict);
            }

            var user = new User
            {
                FirstName = registerDTO.FirstName,
                LastName = registerDTO.LastName,
                Email = registerDTO.Email,
                PasswordHash = PasswordHasher.HashPassword(registerDTO.Password),
                Role = "Customer"
            };

            var registeredUser = await _userRepository.RegisterUser(user);

            var userDTO = new UserDTO
            {
                Id = registeredUser.Id,
                FirstName = registeredUser.FirstName,
                LastName = registeredUser.LastName,
                Email = registeredUser.Email,
                Role = registeredUser.Role
            };

            return Result<UserDTO>.SuccessResult(userDTO);
        }

        public async Task<Result<(string Token, UserDTO User)>> LoginUser(UserLoginDTO loginDTO)
        {
            var user = await _userRepository.GetUserByEmail(loginDTO.Email);

            if (user == null || !PasswordHasher.VerifyPassword(user.PasswordHash, loginDTO.Password))
            {
                return Result<(string Token, UserDTO User)>.Failure("Email or password is incorrect", ErrorType.BadRequest);
            }

            var secretKey = _configuration["Jwt:SecretKey"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var token = JwtHelper.GenerateToken(user, secretKey!, issuer!, audience!);

            var userDTO = new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role
            };

            return Result<(string Token, UserDTO User)>.SuccessResult((token, userDTO));
        }

        public async Task<List<UserDTO>> GetAllUsers()
        {
            var users =  await _userRepository.GetAllUsers();

            return users.Select(u => new UserDTO
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role
            }).ToList();
        }
    }
}
