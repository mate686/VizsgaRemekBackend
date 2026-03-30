using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VizsgaRemekBackend.Dtos.OrderDtos;
using VizsgaRemekBackend.Services.Orders;

namespace VizsgaRemekBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Minden rendeléshez be kell lenni jelentkezve
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{publicid}")]
        public async Task<IActionResult> GetOrder(Guid publicid)
        {
            var order = await _orderService.GetOrderByIdAsync(publicid);
            if (order == null) return NotFound(new { message = "Rendelés nem található." });

            // Biztonság: csak a sajátját láthatja, kivéve ha admin
            if (order.UserId != GetUserId() && !User.IsInRole("Admin")) return Forbid();

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] List<CartItemDto> items)
        {
            var result = await _orderService.CreateOrderAsync(GetUserId(), items);
            if (result != "Sikeres") return BadRequest(new { message = result });

            return Ok(new { message = "Rendelés sikeresen mentve!" });
        }

        [HttpPut("items/{foodPublicId}")]
        public async Task<IActionResult> UpdateQuantity(Guid foodPublicId, [FromQuery] Guid orderId, [FromQuery] int quantity)
        {
            var success = await _orderService.UpdateItemQuantityAsync(orderId, foodPublicId, quantity);
            if (!success) return BadRequest(new { message = "Sikertelen módosítás." });

            return Ok(new { message = "Mennyiség frissítve!" });
        }

        [HttpDelete("items/{foodPublicId}")]
        public async Task<IActionResult> RemoveItem(Guid foodPublicId, [FromQuery] Guid orderId)
        {
            var success = await _orderService.RemoveItemFromOrderAsync(orderId, foodPublicId);
            if (!success) return BadRequest(new { message = "Sikertelen törlés." });

            return Ok(new { message = "Tétel eltávolítva!" });
        }

        // ÚJ: Fizetés és pontbeváltás végpontja
        [HttpPost("{publicid}/checkout")]
        public async Task<IActionResult> Checkout(Guid publicid, [FromQuery] int pointsToUse = 0)
        {
            var userId = GetUserId();
            var result = await _orderService.CheckoutOrderAsync(publicid, userId, pointsToUse);

            if (!result.StartsWith("Sikeres")) return BadRequest(new { message = result });

            return Ok(new { message = result });
        }

        [HttpPatch("{publicid}/status")]
        [Authorize(Roles = "Admin")] // Ezt csak adminok állíthatják
        public async Task<IActionResult> UpdateStatus(Guid publicid, [FromBody] string newStatus)
        {
            var success = await _orderService.UpdateOrderStatusAsync(publicid, newStatus);
            if (!success) return NotFound();

            return Ok(new { message = $"Rendelés státusza frissítve: {newStatus}" });
        }

        [HttpDelete("{publicid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(Guid publicid)
        {
            var success = await _orderService.DeleteOrderAsync(publicid);
            if (!success) return NotFound();

            return Ok(new { message = "Rendelés törölve." });
        }
    }
}