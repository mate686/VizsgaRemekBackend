namespace VizsgaRemekBackend.Models
{
    public class Review
    {
        public int Id { get; set; }
        public Guid PublicId { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public int RestaurantId { get; set; }
        public int Rating { get; set; } 
        public string Comment { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public Restaurant Restaurant { get; set; } = null!;
    }
}
