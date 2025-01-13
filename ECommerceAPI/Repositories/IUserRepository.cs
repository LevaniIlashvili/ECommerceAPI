using ECommerceAPI.Models;

namespace ECommerceAPI.Repositories
{
    public interface IUserRepository
    {
        Task<User> RegisterUser(User user, string password);
        Task<User?> LoginUser(string email, string password);
        Task<User?> GetUserById(int id);
        Task<List<User>> GetAllUsers();
        Task<bool> DeleteUser(int id);
    }
}
