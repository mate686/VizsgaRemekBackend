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
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetAllReviews()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Nem található felhasználói azonosító a tokenben.");
            }
            return Ok(await _irs.GetAllReviewsAsync(userId));
        }


        [HttpPost("createReview")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            /*if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Nem található felhasználói azonosító a tokenben.");
            }*/
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
            string result = await _irs.DeleteReviewAsync(pubid);
            if (result == "Sikeres törlés")
            {
                return Ok(result);
            }
            else if (result == "Nincs ilyen értékelés")
            {
                return NotFound(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet("getReview/{pubid}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetReview(Guid pubid)
        {
            var review = await _irs.GetReviewAsync(pubid);
            if (review != null)
            {
                return Ok(review);
            }
            else
            {
                return NotFound("Nincs ilyen értékelés");
            }
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
