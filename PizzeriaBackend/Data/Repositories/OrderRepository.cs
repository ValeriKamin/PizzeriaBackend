using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PizzeriaBackend.Models.Cart;
using PizzeriaBackend.Models.Orders;
using PizzeriaBackend.Data.Interfaces;

namespace PizzeriaBackend.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly Database _db;

        public OrderRepository(Database db)
        {
            _db = db;
        }

        public int CreateOrder(Order order)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var sql = @"INSERT INTO Orders
        (Username, UserId, Phone, Email, DeliveryType, Address, Apartment, Entrance, Floor, DoorCode, CommentForCourier, DeliveryTime, Total, Status, CreatedAt, Name)
        VALUES
        (@username, @userId, @phone, @email, @deliveryType, @address, @apartment, @entrance, @floor, @doorCode, @comment, @time, @total, @status, @created, @name)";

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
            cmd.Parameters.AddWithValue("@name", order.Name);
            cmd.Parameters.AddWithValue("@userId", order.UserId);

            cmd.ExecuteNonQuery();
            return (int)cmd.LastInsertedId;
        }

        public List<Order> GetAllOrders()
        {
            return new List<Order>();
        }

        public void UpdateStatus(int orderId, string newStatus)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand("UPDATE Orders SET Status = @status WHERE OrderId = @id", conn);
            cmd.Parameters.AddWithValue("@status", newStatus);
            cmd.Parameters.AddWithValue("@id", orderId);
            cmd.ExecuteNonQuery();
        }

        public List<OrderWithItems> GetOrdersByUser(string username)
        {
            var orders = new List<OrderWithItems>();

            using var conn = _db.GetConnection();
            conn.Open();

            var sql = "SELECT * FROM Orders WHERE Username = @user ORDER BY CreatedAt DESC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@user", username);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                orders.Add(new OrderWithItems
                {
                    Id = Convert.ToInt32(reader["OrderId"]),
                    Username = reader["Username"].ToString(),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                    Status = reader["Status"].ToString(),
                    Total = Convert.ToDecimal(reader["Total"]),
                    Items = new List<OrderItem>(),
                    DeliveryTime = reader["DeliveryTime"].ToString()
                });
            }
            reader.Close();

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

        public List<OrderWithItems> GetOrdersWithItemsByUser(string username)
        {
            var orders = new List<OrderWithItems>();

            using var conn = _db.GetConnection();
            conn.Open();

            var sql = "SELECT * FROM Orders WHERE Username = @username ORDER BY CreatedAt DESC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", username);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var order = new OrderWithItems
                {
                    Id = Convert.ToInt32(reader["OrderId"]),
                    Username = reader["Username"].ToString(),
                    Phone = reader["Phone"].ToString(),
                    Email = reader["Email"].ToString(),
                    DeliveryType = reader["DeliveryType"].ToString(),
                    Address = reader["Address"].ToString(),
                    Apartment = reader["Apartment"].ToString(),
                    Entrance = reader["Entrance"].ToString(),
                    Floor = reader["Floor"].ToString(),
                    DoorCode = reader["DoorCode"].ToString(),
                    CourierComment = reader["CommentForCourier"].ToString(),
                    Total = Convert.ToDecimal(reader["Total"]),
                    Status = reader["Status"].ToString(),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                    Items = new List<OrderItem>(),
                    Name = reader["Name"].ToString(),
                    DeliveryTime = reader["DeliveryTime"].ToString()
                };
                orders.Add(order);
            }
            reader.Close();

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
                    Id = Convert.ToInt32(reader["OrderId"]),
                    Username = reader["Username"].ToString(),
                    Phone = reader["Phone"].ToString(),
                    Email = reader["Email"].ToString(),
                    DeliveryType = reader["DeliveryType"].ToString(),
                    Address = reader["Address"].ToString(),
                    Apartment = reader["Apartment"].ToString(),
                    Entrance = reader["Entrance"].ToString(),
                    Floor = reader["Floor"].ToString(),
                    DoorCode = reader["DoorCode"].ToString(),
                    CourierComment = reader["CommentForCourier"].ToString(),
                    Total = Convert.ToDecimal(reader["Total"]),
                    Status = reader["Status"].ToString(),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                    DeliveryTime = reader["DeliveryTime"].ToString(),
                    Name = reader["Name"].ToString()
                });
            }

            return orders;
        }



        public List<OrderWithItems> GetOrdersWithItemsByStatus(string status)
        {
            var orders = new List<OrderWithItems>();

            using var conn = _db.GetConnection();
            conn.Open();

            var sql = "SELECT * FROM Orders WHERE Status = @status ORDER BY CreatedAt DESC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@status", status);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var order = new OrderWithItems
                {
                    Id = Convert.ToInt32(reader["OrderId"]),
                    Username = reader["Username"].ToString(),
                    Phone = reader["Phone"].ToString(),
                    Email = reader["Email"].ToString(),
                    DeliveryType = reader["DeliveryType"].ToString(),
                    Address = reader["Address"].ToString(),
                    Apartment = reader["Apartment"].ToString(),
                    Entrance = reader["Entrance"].ToString(),
                    Floor = reader["Floor"].ToString(),
                    DoorCode = reader["DoorCode"].ToString(),
                    CourierComment = reader["CommentForCourier"].ToString(),
                    Total = Convert.ToDecimal(reader["Total"]),
                    Status = reader["Status"].ToString(),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                    Items = new List<OrderItem>(),
                    Name = reader["Name"].ToString(),
                    DeliveryTime = reader["DeliveryTime"].ToString()
                };
                orders.Add(order);
            }
            reader.Close();


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

        public void AddOrderItems(int orderId, List<CartItem> items)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var groupedItems = items
                .GroupBy(i => i.FoodId)
                .Select(g => new {
                    FoodId = g.Key,
                    FoodName = g.First().FoodName,
                    Quantity = g.Sum(i => i.Quantity)
                });

            foreach (var item in groupedItems)
            {
                var sql = "INSERT INTO OrderItems (OrderId, FoodId, FoodName, Quantity) VALUES (@orderId, @foodId, @name, @qty)";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@orderId", orderId);
                cmd.Parameters.AddWithValue("@foodId", item.FoodId);
                cmd.Parameters.AddWithValue("@name", item.FoodName);
                cmd.Parameters.AddWithValue("@qty", item.Quantity);
                cmd.ExecuteNonQuery();
            }
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
                    Items = new List<OrderItem>()
                };
                orders.Add(order);
            }
            reader.Close();

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

        public List<string> GetBusyDeliveryTimes()
        {
            var times = new List<string>();
            using var conn = _db.GetConnection();
            conn.Open();

            var cmd = new MySqlCommand("SELECT DISTINCT DeliveryTime FROM Orders", conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var rawValue = reader["DeliveryTime"]?.ToString();
                if (!string.IsNullOrWhiteSpace(rawValue))
                times.Add(rawValue);
            }

            return times;
        }
    }
}
