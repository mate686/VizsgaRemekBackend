using System.ComponentModel.DataAnnotations;

namespace VizsgaRemekBackend.Controllers.Orders
{
    public class UpdateOrderStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}
