using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VizsgaRemekBackend.Dtos.RestaurantDtos;
using VizsgaRemekBackend.Services.Restaurants;

namespace VizsgaRemekBackend.Controllers.Restaurant
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _irs;

        public RestaurantController(IRestaurantService irs)
        {
            _irs = irs;
        }

        [HttpGet("allRestaurant")]
        public async Task<IActionResult> GetAllRestaurants()
        {

            return Ok(await _irs.GetAllRestaurantAsync());
        }

        [HttpGet("getRestaurant/{pubid}")]
        public async Task<IActionResult> GetRestaurantById(Guid pubid)
        {
            GetRestaurantDto restaurant = await _irs.GetRestaurantByIdAsnyc(pubid);
            if (restaurant == null)
            {
                return NotFound("Nincs iylen étterem");
            }
            return Ok(restaurant);
        }
    }
}
