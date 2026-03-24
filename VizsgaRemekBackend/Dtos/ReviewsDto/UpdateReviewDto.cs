using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.Dtos.ReviewsDto
{
    public class UpdateReviewDto
    {

        public Guid PublicId { get; set; } 
        public int Rating { get; set; }
        public string Comment { get; set; } = null!;


    }
}
