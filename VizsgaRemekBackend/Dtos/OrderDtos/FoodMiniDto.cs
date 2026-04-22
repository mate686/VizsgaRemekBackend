namespace VizsgaRemekBackend.Dtos.OrderDtos
{
    public class FoodMiniDto
    {
        public Guid PublicId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
    }
}
