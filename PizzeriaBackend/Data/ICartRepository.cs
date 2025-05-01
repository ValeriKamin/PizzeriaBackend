using PizzeriaBackend.Models;
using System.Collections.Generic;

namespace PizzeriaBackend.Data
{
    public interface ICartRepository
    {
        void AddItem(CartItem item);
        List<CartItem> GetItemsByUser(string username);
        void ClearCart(string username);
    }
}