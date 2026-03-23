namespace VizsgaRemekBackend.Dtos.RestaurantDtos
{
    public class CreateRestaurantDto
    {
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string OpeningHours { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string RestaurantImageUrl { get; set; } = null!;
    }
}
