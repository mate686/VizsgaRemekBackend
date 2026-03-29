using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VizsgaRemekBackend.Models;
using VizsgaRemekBackend.Services.Orders;

namespace VizsgaRemekBackend.Controllers.Orders
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{publicId}")]
        public async Task<IActionResult> GetOrderById(Guid publicId)
        {
            var order = await _orderService.GetOrderByIdAsync(publicId);
            if (order == null) return NotFound(new { message = "Rendelés nem található." });

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] List<CartItemDto> items)
        {
            var userId = GetUserId();
            var resultMessage = await _orderService.CreateOrderAsync(userId, items);

            if (resultMessage != "Sikeres")
                return BadRequest(new { message = resultMessage });

            return Ok(new { message = "A tételek sikeresen bekerültek a rendelésbe!" });
        }

        [HttpPatch("{publicId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid publicId, [FromBody] string newStatus)
        {
            var success = await _orderService.UpdateOrderStatusAsync(publicId, newStatus);
            if (!success) return NotFound(new { message = "Rendelés nem található." });

            return Ok(new { message = $"Rendelés státusza frissítve erre: {newStatus}" });
        }


        [HttpDelete("{publicId}")]
        public async Task<IActionResult> DeleteOrder(Guid publicId)
        {
            var success = await _orderService.DeleteOrderAsync(publicId);
            if (!success) return NotFound(new { message = "Rendelés nem található." });

            return Ok(new { message = "Rendelés sikeresen törölve." });
        }

        [HttpPut("{orderPublicId}/items/{foodPublicId}")]
        public async Task<IActionResult> UpdateItemQuantity(Guid orderPublicId, Guid foodPublicId, [FromQuery] int quantity)
        {
            var success = await _orderService.UpdateItemQuantityAsync(orderPublicId, foodPublicId, quantity);
            if (!success) return BadRequest(new { message = "Nem sikerült módosítani a mennyiséget. (Lehet, hogy már nem 'pending' a rendelés)" });

            return Ok(new { message = "Mennyiség frissítve, végösszeg újraszámolva." });
        }

        [HttpDelete("{orderPublicId}/items/{foodPublicId}")]
        public async Task<IActionResult> RemoveItem(Guid orderPublicId, Guid foodPublicId)
        {
            var success = await _orderService.RemoveItemFromOrderAsync(orderPublicId, foodPublicId);
            if (!success) return BadRequest(new { message = "Nem sikerült törölni a tételt." });

            return Ok(new { message = "Tétel sikeresen eltávolítva." });
        }
    }
}
