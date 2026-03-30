using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VizsgaRemekBackend.Data;
using VizsgaRemekBackend.Dtos.ReviewsDto;
using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.Services.Reviews
{
    public class ReviewService : IReviewsService
    {
        private readonly AppDbContext _conn;

        public ReviewService(AppDbContext conn)
        {
            _conn = conn;
        }

        public async Task<string> CreateReviewAsync(CreateReviewDto dto, string userId)
        {

                Review newR = new Review
                {
                    RestaurantId = dto.RestaurantId,
                    Rating = dto.Rating,
                    Comment = dto.Comment,
                    UserId = userId
                };

                _conn.Reviews.Add(newR);

            
                await _conn.SaveChangesAsync();

                return "Sikeres értékelés létrehozás";
            

        }

        public async Task<string> DeleteReviewAsync(Guid pubid)
        {
                Review? r = await _conn.Reviews.FirstOrDefaultAsync(x => x.PublicId == pubid);

                if (r == null)
                {
                    return "Nincs ilyen értékelés";
                }

                _conn.Reviews.Remove(r);


                await _conn.SaveChangesAsync();

                return "Sikeres törlés";
                
        }

        public async Task<List<OutReviewDto>> GetAllReviewsAsync(string userId)
        {
            List<Review> allReviews = await _conn.Reviews.Where(x => x.UserId == userId).ToListAsync();

            List<OutReviewDto> allReviewsDto =  allReviews.Select(x => new OutReviewDto
            {
                Rating = x.Rating,
                Comment = x.Comment,
                publicId = x.PublicId
            }).ToList();
            
            return allReviewsDto;
        }

        public async Task<UpdateReviewDto?> GetReviewAsync(Guid pubid)
        {
            UpdateReviewDto? review =await _conn.Reviews.Where(x => x.PublicId == pubid).Select(x => new UpdateReviewDto
            {
                PublicId = x.PublicId,
                Rating = x.Rating,
                Comment = x.Comment
            }).FirstOrDefaultAsync();

            return review;
        }

        /*public async Task<string> UpdateReviewAsync(string userpubid, UpdateReviewDto dto)
        {
            
                Review? r = await _conn.Reviews.FirstOrDefaultAsync(x => x.PublicId.ToString() == userpubid && x.UserId == userpubid);
                if (r == null) {
                    return "Nincs ilyen értékelés";
                }
                r.Rating = dto.Rating;
                r.Comment = dto.Comment;
                r.UpdatedAt = DateTime.Now;


                await _conn.SaveChangesAsync();

                return "Sikeres értékelés frissítés";
            
        }*/

        public async Task<bool> UpdateReviewAsync(string userId, UpdateReviewDto dto)
        {

            var review = await _conn.Reviews.FirstOrDefaultAsync(x => x.PublicId == dto.PublicId && x.UserId == userId);

            if (review == null) return false;

            review.Rating = dto.Rating;
            review.Comment = dto.Comment;
            review.UpdatedAt = DateTime.UtcNow;

            return await _conn.SaveChangesAsync() > 0;
        }
    }
}
