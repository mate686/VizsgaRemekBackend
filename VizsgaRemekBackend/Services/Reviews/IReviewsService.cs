using VizsgaRemekBackend.Dtos.ReviewsDto;
using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.Services.Reviews
{
    public interface IReviewsService
    {
        Task<List<OutReviewDto>> GetAllReviewsAsync(string userId);
        Task<string> CreateReviewAsync(CreateReviewDto dto, string userId);
        Task<string> DeleteReviewAsync(Guid pubid);
        Task<string> UpdateReviewAsync(string userpubid, UpdateReviewDto dto);
        Task<UpdateReviewDto> GetReviewAsync(Guid pubid);
    }
}
