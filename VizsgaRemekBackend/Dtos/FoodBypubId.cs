using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.Dtos
{
    public class FoodBypubId
    {
        public Guid publicId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string Category { get; set; } = null!;

        public ICollection<FoodImage> Images { get; set; } = new List<FoodImage>();
    }
}
