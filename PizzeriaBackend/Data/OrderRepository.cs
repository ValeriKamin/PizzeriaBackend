using PizzeriaBackend.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace PizzeriaBackend.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly Database _db;

        public OrderRepository(Database db)
        {
            _db = db;
        }

        public void CreateOrder(Order order)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var sql = @"INSERT INTO Orders
            (Username , Phone, Email, DeliveryType, Address, Apartment, Entrance, Floor, DoorCode, CommentForCourier, DeliveryTime,Total, Status, CreatedAt)
            VALUES
            (@username, @phone, @email, @deliveryType, @address, @apartment, @entrance, @floor, @doorCode, @comment, @time, @total, @status, @created)";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", order.Username);
            cmd.Parameters.AddWithValue("@phone", order.Phone);
            cmd.Parameters.AddWithValue("@email", order.Email);
            cmd.Parameters.AddWithValue("@deliveryType", order.DeliveryType);
            cmd.Parameters.AddWithValue("@address", order.Address);
            cmd.Parameters.AddWithValue("@apartment", order.Apartment);
            cmd.Parameters.AddWithValue("@entrance", order.Entrance);
            cmd.Parameters.AddWithValue("@floor", order.Floor);
            cmd.Parameters.AddWithValue("@doorCode", order.DoorCode);
            cmd.Parameters.AddWithValue("@comment", order.CourierComment);
            cmd.Parameters.AddWithValue("@time", order.DeliveryTime);
            cmd.Parameters.AddWithValue("@total", order.Total);
            cmd.Parameters.AddWithValue("@status", order.Status);
            cmd.Parameters.AddWithValue("@created", order.CreatedAt);

            cmd.ExecuteNonQuery();
        }

        public List<Order> GetAllOrders()
        {
            // (можемо додати пізніше)
            return new List<Order>();
        }

        public void UpdateStatus(int orderId, string newStatus)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand("UPDATE Orders SET Status = @status WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@status", newStatus);
            cmd.Parameters.AddWithValue("@id", orderId);
            cmd.ExecuteNonQuery();
        }

        public List<Order> GetOrdersByUser(string username)
        {
            var orders = new List<Order>();

            using var conn = _db.GetConnection();
            conn.Open();

            var sql = "SELECT * FROM Orders WHERE Username = @user ORDER BY CreatedAt DESC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@user", username);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                orders.Add(new Order
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Username = reader["Username"].ToString(),
                    FullName = reader["FullName"].ToString(),
                    Phone = reader["Phone"].ToString(),
                    Email = reader["Email"].ToString(),
                    DeliveryType = reader["DeliveryType"].ToString(),
                    Address = reader["Address"].ToString(),
                    Apartment = reader["Apartment"].ToString(),
                    Entrance = reader["Entrance"].ToString(),
                    Floor = reader["Floor"].ToString(),
                    DoorCode = reader["DoorCode"].ToString(),
                    CourierComment = reader["CourierComment"].ToString(),
                    DeliveryTime = reader["DeliveryTime"].ToString(),
                    CardNumber = reader["CardNumber"].ToString(),
                    CVM = reader["CVM"].ToString(),
                    Expiry = reader["Expiry"].ToString(),
                    Total = Convert.ToDecimal(reader["Total"]),
                    Status = reader["Status"].ToString(),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                });
            }

            return orders;
        }



        public List<Order> GetOrdersByStatus(string status)
        {
            var orders = new List<Order>();

            using var conn = _db.GetConnection();
            conn.Open();

            var sql = "SELECT * FROM Orders WHERE Status = @status ORDER BY CreatedAt DESC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@status", status);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                orders.Add(new Order
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Username = reader["Username"].ToString(),
                    FullName = reader["FullName"].ToString(),
                    Phone = reader["Phone"].ToString(),
                    Email = reader["Email"].ToString(),
                    DeliveryType = reader["DeliveryType"].ToString(),
                    Address = reader["Address"].ToString(),
                    Apartment = reader["Apartment"].ToString(),
                    Entrance = reader["Entrance"].ToString(),
                    Floor = reader["Floor"].ToString(),
                    DoorCode = reader["DoorCode"].ToString(),
                    CourierComment = reader["CourierComment"].ToString(),
                    DeliveryTime = reader["DeliveryTime"].ToString(),
                    CardNumber = reader["CardNumber"].ToString(),
                    CVM = reader["CVM"].ToString(),
                    Expiry = reader["Expiry"].ToString(),
                    Total = Convert.ToDecimal(reader["Total"]),
                    Status = reader["Status"].ToString(),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                });
            }

            return orders;
        }


        public List<OrderWithItems> GetOrdersByUsername(string username)
        {
            var orders = new List<OrderWithItems>();

            using var conn = _db.GetConnection();
            conn.Open();

            var orderCmd = new MySqlCommand("SELECT * FROM Orders WHERE Username = @username ORDER BY CreatedAt DESC", conn);
            orderCmd.Parameters.AddWithValue("@username", username);

            using var reader = orderCmd.ExecuteReader();
            while (reader.Read())
            {
                var order = new OrderWithItems
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                    Status = reader["Status"].ToString(),
                    Total = Convert.ToDecimal(reader["Total"]),
                    Items = new List<OrderItem>() // додаємо пізніше
                };
                orders.Add(order);
            }
            reader.Close();

            // Тепер додаємо товари до кожного замовлення
            foreach (var order in orders)
            {
                var itemCmd = new MySqlCommand("SELECT * FROM OrderItems WHERE OrderId = @id", conn);
                itemCmd.Parameters.AddWithValue("@id", order.Id);

                using var itemReader = itemCmd.ExecuteReader();
                while (itemReader.Read())
                {
                    order.Items.Add(new OrderItem
                    {
                        FoodName = itemReader["FoodName"].ToString(),
                        Quantity = Convert.ToInt32(itemReader["Quantity"])
                    });
                }
                itemReader.Close();
            }

            return orders;
        }
    }
}
