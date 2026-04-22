namespace VizsgaRemekBackend.Dtos.OrderDtos
{
    public class OrderItemResponseDto
    {
        public int Id { get; set; }
        public int FoodId { get; set; }
        public int Quantity { get; set; }
        public int RestaurantId { get; set; }

        public FoodMiniDto? Food { get; set; }
    }
}
