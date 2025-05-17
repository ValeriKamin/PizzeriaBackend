using PizzeriaBackend.Models.Cart;
using PizzeriaBackend.Models.Orders;
using System.Collections.Generic;
namespace PizzeriaBackend.Data.Interfaces
{
    public interface IOrderRepository
    {
        int CreateOrder(Order order);
        List<Order> GetAllOrders();
        void UpdateStatus(int orderId, string newStatus);
        List<Order> GetOrdersByStatus(string status);
        List<OrderWithItems> GetOrdersByUsername(string username);
        List<OrderWithItems> GetOrdersWithItemsByStatus(string status);
        void AddOrderItems(int orderId, List<CartItem> items);
        List<OrderWithItems> GetOrdersWithItemsByUser(string username);
        List<OrderWithItems> GetOrdersByUser(string username);
    }

}
