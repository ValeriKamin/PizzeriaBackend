using PizzeriaBackend.Models;
using System.Collections.Generic;
namespace PizzeriaBackend.Data
{
    public interface IOrderRepository
    {
        void CreateOrder(Order order);
        List<Order> GetAllOrders();
        void UpdateStatus(int orderId, string newStatus);

        List<Order> GetOrdersByUser(string username);

        List<Order> GetOrdersByStatus(string status);
        List<OrderWithItems> GetOrdersByUsername(string username);

    }

}
