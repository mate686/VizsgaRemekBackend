using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VizsgaRemekBackend.Dtos.RestaurantDtos;
using VizsgaRemekBackend.Services.Restaurants;

namespace VizsgaRemekBackend.Controllers.Restaurant
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _irs;

        public RestaurantController(IRestaurantService irs)
        {
            _irs = irs;
        }

        [HttpGet("allRestaurant")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllRestaurants()
        {

            return Ok(await _irs.GetAllRestaurantAsync());
        }

        [HttpGet("getRestaurant/{pubid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRestaurantById(Guid pubid)
        {
            GetRestaurantDto restaurant = await _irs.GetRestaurantByIdAsync(pubid);
            if (restaurant == null)
            {
                return NotFound("Nincs iylen étterem");
            }
            return Ok(restaurant);
        }

        [HttpPost("createRestaurant")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRestaurant([FromBody] CreateRestaurantDto dto)
        {
            if (await _irs.CreateRestaurantAsync(dto))
            {
        
                return Ok("Sikeres létrehozás");
            }
            return BadRequest("Nem sikerült létrehozni az éttermet");


        }

        [HttpDelete("deleteRestaurant/{pubid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRestaurant(Guid pubid)
        {
            bool result = await _irs.DeleteRestaurantAsync(pubid);
            if (result)
            {
                return Ok(result);
            }
            else if (!result)
            {
                return NotFound(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpGet("getupdateRestaurant/{pubid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUpdateRestaurant(Guid pubid)
        {
            CreateRestaurantDto restaurant = await _irs.GetUpdateRestaurantAsync(pubid);
            if (restaurant == null)
            {
                return NotFound("Nincs iylen étterem");
            }
            return Ok(restaurant);

        }

        [HttpPatch("updateRestaurant/{pubid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRestaurant(Guid pubid, [FromBody] CreateRestaurantDto dto)
        {
            bool result = await _irs.UpdateRestaurantAsync(pubid, dto);
            if (result)
            {
                return Ok(result);
            }
            else if (!result)
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
