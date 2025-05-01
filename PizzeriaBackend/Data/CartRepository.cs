using PizzeriaBackend.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace PizzeriaBackend.Data
{
    public class CartRepository : ICartRepository
    {
        private readonly Database _db;

        public CartRepository(Database db)
        {
            _db = db;
        }

        public void AddItem(CartItem item)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var sql = @"INSERT INTO CartItems (FoodId, FoodName, Quantity, Weight, Price, Username)
                        VALUES (@foodId, @name, @qty, @weight, @price, @user)";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@foodId", item.FoodId);
            cmd.Parameters.AddWithValue("@name", item.FoodName);
            cmd.Parameters.AddWithValue("@qty", item.Quantity);
            cmd.Parameters.AddWithValue("@weight", item.Weight);
            cmd.Parameters.AddWithValue("@price", item.Price);
            cmd.Parameters.AddWithValue("@user", item.Username);

            cmd.ExecuteNonQuery();
        }

        public List<CartItem> GetItemsByUser(string username)
        {
            var items = new List<CartItem>();
            using var conn = _db.GetConnection();
            conn.Open();

            var sql = "SELECT * FROM CartItems WHERE Username = @user";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@user", username);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                items.Add(new CartItem
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    FoodId = Convert.ToInt32(reader["FoodId"]),
                    FoodName = reader["FoodName"].ToString(),
                    Quantity = Convert.ToInt32(reader["Quantity"]),
                    Weight = Convert.ToDouble(reader["Weight"]),
                    Price = Convert.ToDecimal(reader["Price"]),
                    Username = reader["Username"].ToString()
                });
            }

            return items;
        }

        public void ClearCart(string username)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var sql = "DELETE FROM CartItems WHERE Username = @user";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@user", username);
            cmd.ExecuteNonQuery();
        }
    }
}