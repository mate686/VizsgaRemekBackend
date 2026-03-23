using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VizsgaRemekBackend.Services.Reviews;

namespace VizsgaRemekBackend.Controllers.Review
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewsService _irs;

        public ReviewController(IReviewsService irs)
        {
            _irs = irs;
        }
    }
}
