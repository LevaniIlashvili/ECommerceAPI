using ECommerceAPI.DTOs;
using ECommerceAPI.Helpers;

namespace ECommerceAPI.Services
{
    public interface IUserService
    {
        Task<Result<UserDTO>> RegisterUser(UserRegisterDTO registerDTO);
        Task<Result<(string Token, UserDTO User)>> LoginUser(UserLoginDTO loginDTO);
        Task<List<UserDTO>> GetAllUsers();
    }
}