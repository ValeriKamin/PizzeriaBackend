using MySql.Data.MySqlClient;
using Pizzeria.Models;
using PizzeriaBackend.Data;

namespace Pizzeria.Data
{
    //[Authorize(Roles = "User")]
    public class FoodRepository
    {
        private readonly Database _db;

        public FoodRepository(Database db)
        {
            _db = db;
        }

        public List<Food> GetAllFoods()
        {
            List<Food> list = new List<Food>();
            using var conn = _db.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM Foods";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Food
                {
                    FoodId = reader.GetInt32("FoodId"),
                    Name = reader.GetString("Name"),
                    Description = reader.GetString("Description"),
                    WeightGrams = reader.GetInt32("WeightGrams"),
                    Price = reader.GetDecimal("Price")
                });
            }

            return list;
        }
    }
}
