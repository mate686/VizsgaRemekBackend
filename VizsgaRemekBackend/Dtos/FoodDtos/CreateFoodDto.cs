using System.ComponentModel.DataAnnotations;
using VizsgaRemekBackend.Dtos.FoodImagesDto;
using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.Dtos.FoodDtos
{
    public class CreateFoodDto
    {

        //public int RestaurantId { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Description { get; set; } = null!;
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Category { get; set; } = null!;

        [Required]
        public Guid RestaurantPublicId { get; set; }

        [Required]
        public List<FoodImageDto> Images { get; set; } = new List<FoodImageDto>();

        //public Restaurant Restaurant { get; set; } = null!;
        //[Required]
        //public ICollection<FoodImage> Images { get; set; } = new List<FoodImage>();
  
        

    }
}
