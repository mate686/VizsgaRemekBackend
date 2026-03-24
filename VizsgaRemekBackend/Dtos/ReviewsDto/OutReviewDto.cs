using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.Dtos.ReviewsDto
{
    public class OutReviewDto
    {
        public Guid publicId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = null!;

    }
}
