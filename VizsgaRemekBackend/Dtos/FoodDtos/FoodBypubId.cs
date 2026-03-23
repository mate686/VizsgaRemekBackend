using VizsgaRemekBackend.Dtos.FoodImagesDto;
using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.Dtos.FoodDtos
{
    public class FoodBypubId
    {
        public Guid publicId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string Category { get; set; } = null!;

        public ICollection<FoodImageDto> Images { get; set; } = new List<FoodImageDto>();
    }
}
