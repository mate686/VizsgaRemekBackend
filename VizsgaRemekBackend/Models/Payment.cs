namespace VizsgaRemekBackend.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string PaymentMethod { get; set; } = null!; // card, cash
        public string PaymentStatus { get; set; } = "pending"; // pending, paid, failed
        public DateTime? PaidAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Order Order { get; set; } = null!;
    }
}
