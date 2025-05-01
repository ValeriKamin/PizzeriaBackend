using PizzeriaBackend.Models;

namespace PizzeriaBackend.Data
{
    public interface IReviewRepository
    {
        void AddReview(Review review);
        List<Review> GetAllReviews();
    }
}