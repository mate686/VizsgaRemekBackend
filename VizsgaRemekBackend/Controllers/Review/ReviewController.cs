using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VizsgaRemekBackend.Dtos.ReviewsDto;
using VizsgaRemekBackend.Services.Reviews;

namespace VizsgaRemekBackend.Controllers.Review
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewsService _irs;

        public ReviewController(IReviewsService irs)
        {
            _irs = irs;
        }

        [HttpGet("allReview")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllReviews(Guid restaurantPubId)
        {
            var reviews = await _irs.GetReviewsByRestaurantAsync(restaurantPubId);
            return Ok(reviews);
        }


        [HttpPost("createReview")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Nem található felhasználói azonosító a tokenben.");
            }
            string result = await _irs.CreateReviewAsync(dto, userId);
            if (result == "Sikeres értékelés létrehozás")
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpDelete("deleteReview/{pubid}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> DeleteReview(Guid pubid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Nem található felhasználói azonosító a tokenben.");
            }

            var isAdmin = User.IsInRole("Admin");

            var result = await _irs.DeleteReviewAsync(pubid, userId, isAdmin);

            return result switch
            {
                DeleteReviewResult.Success => NoContent(),
                DeleteReviewResult.NotFound => NotFound("Nincs ilyen értékelés"),
                DeleteReviewResult.Forbidden => Forbid(),
                _ => StatusCode(500, "Váratlan hiba történt")
            };
        }


        [HttpGet("getReview/{pubid}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetReview(Guid pubid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Nem található felhasználói azonosító a tokenben.");
            }

            var isAdmin = User.IsInRole("Admin");

            var review = await _irs.GetReviewAsync(pubid, userId, isAdmin);

            if (review == null)
            {
                return NotFound("Nincs ilyen értékelés");
            }

            return Ok(review);
        }


        [HttpPatch("updateReview")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> UpdateReview([FromBody] UpdateReviewDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool result = await _irs.UpdateReviewAsync(userId, dto);
            if (result)
            {
                return Ok();
            }
            else 
            {
                return NotFound();
            }
        }
    }
}
