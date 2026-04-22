namespace VizsgaRemekBackend.Dtos.OrderDtos
{
    public class OrderResponseDto
    {
        
        public Guid PublicId { get; set; }
        public string UserId { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public List<OrderItemResponseDto> OrderItems { get; set; } = new();
        
    }
}
