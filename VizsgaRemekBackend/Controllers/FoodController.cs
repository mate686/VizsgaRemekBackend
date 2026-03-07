using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult GetFoods()
        {
            return Ok(_fs.GetAllFood());
        }
    }
}
