using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using VizsgaRemekBackend.Dtos;
using VizsgaRemekBackend.Models;
using VizsgaRemekBackend.Services;

namespace VizsgaRemekBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        private readonly IFoodService _fs;

        public FoodController(IFoodService fs)
        {
            _fs = fs;
        }

        [HttpGet("allfood")]
        public async Task<IActionResult> GetFoods()
        {
            return Ok(await _fs.GetAllFoodAsync());
        }

        [HttpGet("{pubid}")]
        public IActionResult GetFoodById(Guid pubid)
        {
            var food = _fs.GetFoodByIdAsnyc(pubid);
            if (food == null)
            {
                return NotFound();
            }
            return Ok(food);
        }

        [HttpPost]
        public IActionResult CreateFood([FromBody] CreateFoodDto cfood)
        {
            string result = _fs.CreateFoodAsnyc(cfood).ToString();

            if (result == "Sikeres feltöltés")
            {
                return Created();
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPatch("update/{pubid}")]
        public async Task<IActionResult> UpdateFood(Guid pubid, [FromBody] UpdateFoodDto ufood)
        {
            string result = await _fs.UpdateFoodAsnyc(pubid, ufood);

            if (result == "Sikeres módosítás")
            {
                return Ok(result);
            }
            else if (result == "Nem található ilyen étel")
            {
                return NotFound(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpDelete("delete/{pubid}")]
        public async Task<IActionResult> DeleteFood(Guid pubid)
        {
            string result = await _fs.DeleteFoodAsnyc(pubid);

            if (result == "Sikeres törlés")
            {
                return Ok(result);
            }
            else if (result == "Nem található ilyen étel")
            {
                return NotFound(result);
            }
            else
            {
                return BadRequest(result);
            }
        }
    }
}
