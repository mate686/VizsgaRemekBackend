using System.ComponentModel.DataAnnotations;

namespace VizsgaRemekBackend.Dtos.OrderDtos
{
    public class CartItemDto
    {
        [Required]
        public Guid FoodPublicId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "A mennyiségnek legalább 1-nek kell lennie.")]
        public int Quantity { get; set; }
    }
}
