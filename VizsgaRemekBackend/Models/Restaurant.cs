namespace VizsgaRemekBackend.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public Guid publicId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string OpeningHours { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string RestaurantImageUrl { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Food> Foods { get; set; } = new List<Food>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
