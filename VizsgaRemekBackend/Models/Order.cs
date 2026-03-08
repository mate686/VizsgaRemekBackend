namespace VizsgaRemekBackend.Models
{
    public class Order
    {
        public int Id { get; set; }
        public Guid publicId { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public int RestaurantId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "pending"; 
        public decimal TotalPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public Restaurant Restaurant { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public Payment Payment { get; set; } = null!;
    }

}
