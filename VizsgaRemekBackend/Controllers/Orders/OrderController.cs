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
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _iod;

        public OrderController(IOrderService iod)
        {
            _iod = iod;
        }

        [HttpGet("allorder")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetOrders()
        {

            return Ok(await _iod.GetAllOrdersAsync());
        }

        [HttpGet("order/{pubid}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetOrdersById(Guid pubid)
        {

            Order sOrdr = await _iod.GetOrderByIdAsync(pubid);

            return Ok(await _iod.GetOrderByIdAsync(pubid));
        }

        [HttpPost("makeOrder")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> MakeOrder([FromBody] List<OrderItem> orderItems)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null) return Unauthorized();
                // Itt lehet hozzáadni az új rendelést az adatbázishoz
                // Például: await _iod.CreateOrderAsync(order);
                return Created();
            }
            catch (Exception ex)
            {
                return BadRequest($"Hiba történt a rendelés létrehozásakor: {ex.Message}");
            }
        }

        [HttpPost("completeOrder")]
         [Authorize(Roles = "Admin")]
         public IActionResult SetCompleteOrder(Guid pubid)
         {
             if (_iod.CompleteOrderAsync(pubid).Result)
             {
                 return Ok("Sikeres rendelés teljesítése");
             }
             else
             {
                 return BadRequest("Sikertelen rendelés teljesítése");
             }
         }

        [HttpPost("deleteOrder/{pubid}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> DeleteOrder(Guid pubid)
        {

            if (await _iod.DeleteOrderAsync(pubid))
            {
                return Ok("Sikeres törlés");
            }
            else
            {
                return BadRequest("Sikertelen törlés");
            }

        }

        [HttpPost("makeOrderPaid/{pubid}")]
        [Authorize(Roles = "User")]
        public IActionResult MakeOrderPaid(Guid pubId)
        {
            if (_iod.MadeOrderPaid(pubId).Result)
            {
                return Ok("Sikeres fizetés");
            }
            else            {
                return BadRequest("Sikertelen fizetés");
            }
        }

    }
}
