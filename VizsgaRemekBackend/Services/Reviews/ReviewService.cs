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
            var restaurant = await _conn.Restaurants
                .FirstOrDefaultAsync(r => r.publicId == dto.RestaurantPublicId);

            if (restaurant == null)
            {
                return "Nincs ilyen étterem";
            }

            Review newR = new Review
            {
                RestaurantId = restaurant.Id,
                Rating = dto.Rating,
                Comment = dto.Comment,
                UserId = userId
            };

            _conn.Reviews.Add(newR);

            await _conn.SaveChangesAsync();

            return "Sikeres értékelés létrehozás";
        }

        public async Task<DeleteReviewResult> DeleteReviewAsync(Guid pubid, string userId, bool isAdmin)
        {
            var review = await _conn.Reviews.FirstOrDefaultAsync(x => x.PublicId == pubid);

            if (review == null)
            {
                return DeleteReviewResult.NotFound;
            }

            if (!isAdmin && review.UserId != userId)
            {
                return DeleteReviewResult.Forbidden;
            }

            _conn.Reviews.Remove(review);
            await _conn.SaveChangesAsync();

            return DeleteReviewResult.Success;
        }

        public async Task<List<OutReviewDto>> GetReviewsByRestaurantAsync(Guid restaurantPubId)
        {
            var restaurant = await _conn.Restaurants
                .FirstOrDefaultAsync(r => r.publicId == restaurantPubId);

            if (restaurant == null)
                return new List<OutReviewDto>();

            var reviews = await _conn.Reviews
            .Include(x => x.User)
            .Where(x => x.RestaurantId == restaurant.Id)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new OutReviewDto
            {
                publicId = x.PublicId,
                Rating = x.Rating,
                Comment = x.Comment,
                UserName = x.User.UserName,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

            return reviews;
        }

        public async Task<UpdateReviewDto?> GetReviewAsync(Guid pubid, string userId, bool isAdmin)
        {
            var review = await _conn.Reviews
                .Where(x => x.PublicId == pubid && x.UserId == userId)
                .Select(x => new UpdateReviewDto
                {
                    PublicId = x.PublicId,
                    Rating = x.Rating,
                    Comment = x.Comment
                })
                .FirstOrDefaultAsync();

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
            var review = await _conn.Reviews
                .FirstOrDefaultAsync(x => x.PublicId == dto.PublicId && x.UserId == userId);

            if (review == null) return false;

            review.Comment = dto.Comment;
            review.Rating = dto.Rating;

            await _conn.SaveChangesAsync();
            return true;
        }
    }
}
