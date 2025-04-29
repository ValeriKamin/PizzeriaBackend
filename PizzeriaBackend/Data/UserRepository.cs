using MySql.Data.MySqlClient;
using Pizzeria.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzeria.Data
{
    public class UserRepository
    {
        private readonly Database _db = new Database();

        public User? GetByUsername(string username)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string sql = "SELECT * FROM Users WHERE Username = @username";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", username);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    UserId = Convert.ToInt32(reader["UserId"]),
                    Username = reader["Username"].ToString(),
                    PasswordHash = reader["PasswordHash"].ToString(),
                    Email = reader["Email"].ToString(),
                    PhoneNumber = reader["PhoneNumber"].ToString(),
                    Role = reader["Role"].ToString()
                };
            }

            return null;
        }
    }
}
