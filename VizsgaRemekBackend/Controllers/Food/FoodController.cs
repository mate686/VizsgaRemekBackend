using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VizsgaRemekBackend.Dtos.FoodDtos;
using VizsgaRemekBackend.Models;
using VizsgaRemekBackend.Services.FoodServices;

namespace VizsgaRemekBackend.Controllers.Food
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FoodsController : ControllerBase
    {
        private readonly IFoodService _foodService;

        public FoodsController(IFoodService foodService)
        {
            _foodService = foodService;
        }

        
        [HttpGet]
        public async Task<ActionResult<List<AllFoodDto>>> GetAll()
        {
            var foods = await _foodService.GetAllFoodAsync();
            return Ok(foods);
        }

  
        [HttpGet("{publicid}")]
        public async Task<ActionResult<FoodBypubId>> GetById(Guid publicid)
        {
            var food = await _foodService.GetFoodByIdAsync(publicid);
            if (food == null) return NotFound(new { message = "Az étel nem található." });

            return Ok(food);
        }

   
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateFoodDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var success = await _foodService.CreateFoodAsync(dto);
            if (!success) return BadRequest(new { message = "Hiba történt a mentés során." });

            return StatusCode(201, new { message = "Étel sikeresen létrehozva!" });
        }

        [HttpPut("{publicid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid publicid, [FromBody] UpdateFoodDto dto)
        {
            var success = await _foodService.UpdateFoodAsync(publicid, dto);
            if (!success) return NotFound(new { message = "Nem sikerült a módosítás. Lehet, hogy az étel nem létezik." });

            return Ok(new { message = "Étel sikeresen frissítve!" });
        }


        [HttpDelete("{publicid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid publicid)
        {
            var success = await _foodService.DeleteFoodAsync(publicid);
            if (!success) return NotFound(new { message = "Az étel nem található, így nem törölhető." });

            return Ok(new { message = "Étel sikeresen törölve!" });
        }
    }
}
