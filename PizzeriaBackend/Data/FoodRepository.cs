using PizzeriaBackend.Models;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace PizzeriaBackend.Data
{
    public class FoodRepository : IFoodRepository
    {
        private readonly Database _db;

        public FoodRepository(Database db)
        {
            _db = db;
        }

        public List<Food> GetAllFoods()
        {
            var foods = new List<Food>();

            using var conn = _db.GetConnection();
            conn.Open();

            string query = "SELECT * FROM Foods ORDER BY Name ASC";
            using var cmd = new MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                foods.Add(new Food
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    Name = reader["Name"].ToString(),
                    Description = reader["Description"].ToString(),
                    Weight = Convert.ToDouble(reader["Weight"]),
                    Price = Convert.ToDecimal(reader["Price"])
                });
            }

            return foods;
        }

        public void UpdatePrice(int foodId, decimal newPrice)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var sql = "UPDATE Foods SET Price = @price WHERE Id = @id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@price", newPrice);
            cmd.Parameters.AddWithValue("@id", foodId);

            cmd.ExecuteNonQuery();
        }
    }
}
