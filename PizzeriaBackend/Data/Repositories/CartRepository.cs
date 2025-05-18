using MySql.Data.MySqlClient;
using System.Collections.Generic;
using PizzeriaBackend.Models.Cart;
using PizzeriaBackend.Data.Interfaces;

namespace PizzeriaBackend.Data.Repositories
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

            // Перевірка чи існує товар у користувача
            var checkCmd = new MySqlCommand("SELECT * FROM CartItems WHERE Username = @user AND FoodId = @foodId", conn);
            checkCmd.Parameters.AddWithValue("@user", item.Username);
            checkCmd.Parameters.AddWithValue("@foodId", item.FoodId);

            using var reader = checkCmd.ExecuteReader();
            if (reader.Read())
            {
                // Якщо вже існує — оновлюємо
                reader.Close();
                var updateCmd = new MySqlCommand("UPDATE CartItems SET Quantity = Quantity + @qty, Price = Price + @price WHERE Username = @user AND FoodId = @foodId", conn);
                updateCmd.Parameters.AddWithValue("@qty", item.Quantity);
                updateCmd.Parameters.AddWithValue("@price", item.Price);
                updateCmd.Parameters.AddWithValue("@user", item.Username);
                updateCmd.Parameters.AddWithValue("@foodId", item.FoodId);
                updateCmd.ExecuteNonQuery();
            }
            else
            {
                reader.Close();
                var insertCmd = new MySqlCommand(@"INSERT INTO CartItems (FoodId, FoodName, Quantity, Weight, Price, Username)
                                           VALUES (@foodId, @name, @qty, @weight, @price, @user)", conn);
                insertCmd.Parameters.AddWithValue("@foodId", item.FoodId);
                insertCmd.Parameters.AddWithValue("@name", item.FoodName);
                insertCmd.Parameters.AddWithValue("@qty", item.Quantity);
                insertCmd.Parameters.AddWithValue("@weight", item.Weight);
                insertCmd.Parameters.AddWithValue("@price", item.Price);
                insertCmd.Parameters.AddWithValue("@user", item.Username);
                insertCmd.ExecuteNonQuery();
            }
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