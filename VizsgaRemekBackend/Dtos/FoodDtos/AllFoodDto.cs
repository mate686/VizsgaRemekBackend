using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.Dtos.FoodDtos
{
    public class AllFoodDto
    {

        public Guid publicId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string Category { get; set; } = null!;

        public List<string> ImageUrl { get; set; }

        //public ICollection<FoodImage> Images { get; set; } = new List<FoodImage>();

    }
}
