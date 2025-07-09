using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Pizzeria.Controllers;
using PizzeriaBackend.Models;
using Microsoft.AspNetCore.Authorization;
using PizzeriaBackend.Models.Reviews;
using PizzeriaBackend.Data.Interfaces;

namespace PizzeriaBackend.Controllers
{
    //[Authorize(Roles = "User")]
    //[Authorize(Roles = "User")]
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewRepository _repo;

        public ReviewsController(IReviewRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("add")]
        public IActionResult AddReview([FromBody] ReviewModel model)
        {
            var review = new Review
            {
                Name = model.Name,
                Topic = model.Topic,
                Comment = model.Comment,
                PhoneNumber = model.PhoneNumber,
                CreatedAt = DateTime.Now
            };

            _repo.AddReview(review);

            return Ok(new ReviewResponse
            {
                Message = "Відгук додано!"
            });
        }

        [HttpGet("all")]
        //[Authorize(Roles = "Admin")]
        public IActionResult GetAllReviews()
        {
            var reviews = _repo.GetAllReviews();
            return Ok(reviews);
        }
    }
}

