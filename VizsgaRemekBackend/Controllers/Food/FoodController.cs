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
    public class FoodController : ControllerBase
    {
        private readonly IFoodService _fs;

        public FoodController(IFoodService fs)
        {
            _fs = fs;
        }

        [HttpGet("allfood")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFoods()
        {
            return Ok(await _fs.GetAllFoodAsync());
        }

        [HttpGet("getfoodbyid/{pubid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFoodById(Guid pubid)
        {
            var food =await _fs.GetFoodByIdAsnyc(pubid);
            if (food == null)
            {
                return NotFound("Nincs iylen étel");
            }
            return Ok(food);
        }

        [HttpPost("createFood")]
        [Authorize(Roles = "Admin")]
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


        [HttpPatch("getupdate/{pubid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUpdateFood(Guid pubid)
        {
            UpdateFoodDto food = await _fs.GetUpdateFoodAsnyc(pubid);
            
            if (food == null)
            {
                return NotFound("Nincs iylen étel");
            }
            return Ok(food);

        }

        [HttpPatch("update/{pubid}")]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
