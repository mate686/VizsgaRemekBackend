using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.Dtos.ReviewsDto
{
    public class CreateReviewDto
    {

        public Guid RestaurantPublicId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = null!;


    }
}

