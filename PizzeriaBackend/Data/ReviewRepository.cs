using PizzeriaBackend.Models;
using MySql.Data.MySqlClient;

namespace PizzeriaBackend.Data
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly Database _db;

        public ReviewRepository(Database db)
        {
            _db = db;
        }


        public void AddReview(Review review)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var sql = @"INSERT INTO Reviews (Name, Topic, Comment, PhoneNumber, CreatedAt)
                        VALUES (@name, @topic, @comment, @phone, @createdAt)";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", review.Name);
            cmd.Parameters.AddWithValue("@topic", review.Topic);
            cmd.Parameters.AddWithValue("@comment", review.Comment);
            cmd.Parameters.AddWithValue("@phone", review.PhoneNumber);
            cmd.Parameters.AddWithValue("@createdAt", review.CreatedAt);

            cmd.ExecuteNonQuery();
        }

        public List<Review> GetAllReviews()
        {
            var reviews = new List<Review>();

            using var conn = _db.GetConnection();
            conn.Open();

            var sql = "SELECT * FROM Reviews ORDER BY CreatedAt DESC";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                reviews.Add(new Review
                {
                    ReviewId = Convert.ToInt32(reader["ReviewId"]),
                    Name = reader["Name"].ToString(),
                    Topic = reader["Topic"].ToString(),
                    Comment = reader["Comment"].ToString(),
                    PhoneNumber = reader["PhoneNumber"].ToString(),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                });
            }

            return reviews;
        }
    }
}
