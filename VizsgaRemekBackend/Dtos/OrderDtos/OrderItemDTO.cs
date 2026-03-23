using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.Dtos.OrdeeDtos
{
    public class OrderItemDTO
    { 
        public int FoodId { get; set; }
        public int Quantity { get; set; }
        public Food Food { get; set; } = null!;
    }
}
