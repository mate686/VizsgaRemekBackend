using System.ComponentModel.DataAnnotations;

namespace VizsgaRemekBackend.Dtos.OrderDtos
{
    public class UpdateOrderStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}
