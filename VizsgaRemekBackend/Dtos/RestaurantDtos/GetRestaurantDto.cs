using VizsgaRemekBackend.Dtos.FoodDtos;

namespace VizsgaRemekBackend.Dtos.RestaurantDtos
{
    public class GetRestaurantDto
    {
        public Guid publicId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string OpeningHours { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string RestaurantImageUrl { get; set; } = null!;

        public ICollection<FoodBypubId> Foods { get; set; } = new List<FoodBypubId>();
        public ICollection<RestaurantReviewsDto> Reviews { get; set; } = new List<RestaurantReviewsDto>();
    }
}
