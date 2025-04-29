using Pizzeria.Models;

namespace PizzeriaBackend.Data
{
    public interface IUserRepository
    {
        User? GetByUsername(string username);
    }
}
