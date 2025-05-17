using PizzeriaBackend.Models.Reviews;

namespace PizzeriaBackend.Data.Interfaces
{
    public interface IReviewRepository
    {
        void AddReview(Review review);
        List<Review> GetAllReviews();
    }
}