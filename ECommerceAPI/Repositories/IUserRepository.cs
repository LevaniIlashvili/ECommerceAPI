using ECommerceAPI.DTOs;
using ECommerceAPI.Models;

namespace ECommerceAPI.Repositories
{
    public interface IUserRepository
    {
        Task<User> RegisterUser(User user);
        Task<User?> GetUserByEmail(string email);
        Task<List<User>> GetAllUsers();
    }
}
