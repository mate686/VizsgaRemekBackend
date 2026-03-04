namespace VizsgaRemekBackend.Models
{
    public class FoodImage
    {
        public int Id { get; set; }
        public int FoodId { get; set; }
        public string ImageUrl { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Food Food { get; set; } = null!;
    }
}
