using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using VizsgaRemekBackend.Controllers.Orders;
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

        [HttpGet("points")]
        public async Task<IActionResult> GetPoints() {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Nem található felhasználói azonosító a tokenben.");
            int points =await _orderService.GetUserPoints(userId);
            return Ok( points );
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Nem található felhasználói azonosító a tokenben.");

            var orders = await _orderService.GetOrdersForUserAsync(userId);
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

        [HttpGet("allAdmin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrdersForAdmin()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] List<CartItemDto> items)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _orderService.CreateOrderAsync(GetUserId(), items);
            if (result != "Sikeres") return BadRequest(new { message = result });

            return Ok(new { message = "Rendelés sikeresen mentve!" });
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(Guid publicid, [FromBody] UpdateOrderStatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(dto.Status))
                return BadRequest(new { message = "A státusz megadása kötelező." });

            var success = await _orderService.UpdateOrderStatusAsync(publicid, dto.Status);

            if (!success)
                return NotFound(new { message = "A rendelés nem található." });

            return Ok(new { message = $"Rendelés státusza frissítve: {dto.Status}" });
        }

        [HttpDelete("{publicid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(Guid publicid)
        {
            var success = await _orderService.DeleteOrderAsync(publicid);
            if (!success) return NotFound();

            return Ok(new { message = "Rendelés törölve." });
        }



        [HttpPut("items/{foodPublicId}")]
        public async Task<IActionResult> UpdateQuantity(Guid foodPublicId, [FromQuery] Guid orderId, [FromQuery] int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Nem található felhasználói azonosító a tokenben.");

            var isAdmin = User.IsInRole("Admin");

            var success = await _orderService.UpdateItemQuantityAsync(orderId, foodPublicId, quantity, userId, isAdmin);

            if (!success)
                return BadRequest(new { message = "Sikertelen módosítás." });

            return Ok(new { message = "Mennyiség frissítve!" });
        }

        [HttpDelete("items/{foodPublicId}")]
        public async Task<IActionResult> RemoveItem(Guid foodPublicId, [FromQuery] Guid orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Nem található felhasználói azonosító a tokenben.");

            var isAdmin = User.IsInRole("Admin");

            var success = await _orderService.RemoveItemFromOrderAsync(orderId, foodPublicId, userId, isAdmin);

            if (!success)
                return BadRequest(new { message = "Sikertelen törlés." });

            return Ok(new { message = "Tétel eltávolítva!" });
        }
    }
}