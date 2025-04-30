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
    }
}
