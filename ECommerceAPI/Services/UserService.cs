using AutoMapper;
using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;

namespace ECommerceAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IConfiguration configuration, IMapper mapper)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<Result<UserDTO>> RegisterUser(UserRegisterDTO registerDTO)
        {
            var userExists = await _userRepository.GetUserByEmail(registerDTO.Email);

            if (userExists != null)
            {
                return Result<UserDTO>.Failure("User with this email already exists", ErrorType.Conflict);
            }

            var user = _mapper.Map<User>(registerDTO);
            user.PasswordHash = PasswordHasher.HashPassword(registerDTO.Password);
            user.Role = "Customer";

            var registeredUser = await _userRepository.RegisterUser(user);

            var userDTO = _mapper.Map<UserDTO>(registeredUser);

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

            var userDTO = _mapper.Map<UserDTO>(user);

            return Result<(string Token, UserDTO User)>.SuccessResult((token, userDTO));
        }

        public async Task<List<UserDTO>> GetAllUsers()
        {
            var users =  await _userRepository.GetAllUsers();

            return _mapper.Map<List<UserDTO>>(users);
        }
    }
}
