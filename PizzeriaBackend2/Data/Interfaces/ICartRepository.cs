using PizzeriaBackend.Models.Cart;
using System.Collections.Generic;

namespace PizzeriaBackend.Data.Interfaces
{
    public interface ICartRepository
    {
        void AddItem(CartItem item);
        List<CartItem> GetItemsByUser(string username);
        void ClearCart(string username);
    }
}