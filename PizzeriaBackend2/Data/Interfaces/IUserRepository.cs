using Pizzeria.Models;

namespace PizzeriaBackend.Data.Interfaces
{
    public interface IUserRepository
    {
        User? GetByUsername(string username);
        void CreateUser(User user);
        User? GetByEmail(string email);
    }
}
