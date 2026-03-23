using VizsgaRemekBackend.Dtos.UserDtos;

namespace VizsgaRemekBackend.Dtos.RestaurantDtos
{
    public class RestaurantReviewsDto
    {
        public int Rating { get; set; }
        public string Comment { get; set; } = null!;

        public UserDto User { get; set; } = null!;
    }
}
