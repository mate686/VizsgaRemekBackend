namespace VizsgaRemekBackend.Dtos.RestaurantDtos
{
    public class AllRestaurantDto
    {
        public Guid publicId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string OpeningHours { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string RestaurantImageUrl { get; set; } = null!;
    }
}
