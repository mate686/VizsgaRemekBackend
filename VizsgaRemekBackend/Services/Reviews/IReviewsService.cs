using VizsgaRemekBackend.Dtos.ReviewsDto;
using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.Services.Reviews
{
    public interface IReviewsService
    {
        Task<List<OutReviewDto>> GetReviewsByRestaurantAsync(Guid restaurantPubId);
        Task<string> CreateReviewAsync(CreateReviewDto dto, string userId);
        Task<DeleteReviewResult> DeleteReviewAsync(Guid pubid, string userId, bool isAdmin);
        Task<bool> UpdateReviewAsync(string userId, UpdateReviewDto dto);
        Task<UpdateReviewDto?> GetReviewAsync(Guid pubid, string userId, bool isAdmin);
    }
}
