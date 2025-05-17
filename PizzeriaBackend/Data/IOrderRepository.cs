using PizzeriaBackend.Models;
using System.Collections.Generic;
namespace PizzeriaBackend.Data
{
    public interface IOrderRepository
    {
        int CreateOrder(Order order);
        List<Order> GetAllOrders();
        void UpdateStatus(int orderId, string newStatus);

        //List<Order> GetOrdersByUser(string username);

        List<Order> GetOrdersByStatus(string status);
        List<OrderWithItems> GetOrdersByUsername(string username);
        List<OrderWithItems> GetOrdersWithItemsByStatus(string status);
        void AddOrderItems(int orderId, List<CartItem> items);
        List<OrderWithItems> GetOrdersWithItemsByUser(string username);

        List<OrderWithItems> GetOrdersByUser(string username);
    }

}
