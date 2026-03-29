using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VizsgaRemekBackend.Services.Favorites;

namespace VizsgaRemekBackend.Controllers.Favorite
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyFavorites()
        {
            var userId = GetUserId();
            var favorites = await _favoriteService.GetUserFavoritesAsync(userId);
            return Ok(favorites);
        }

        [HttpPost("{foodPublicId}")]
        public async Task<IActionResult> AddToFavorites(Guid foodPublicId)
        {
            var userId = GetUserId();
            var success = await _favoriteService.AddToFavoritesAsync(userId, foodPublicId);

            if (!success)
            {
                return BadRequest(new { message = "Nem sikerült hozzáadni a kedvencekhez." });
            }

            return Ok(new { message = "Sikeresen hozzáadva a kedvencekhez." });
        }

        [HttpDelete("{foodPublicId}")]
        public async Task<IActionResult> RemoveFromFavorites(Guid foodPublicId)
        {
            var userId = GetUserId();
            var success = await _favoriteService.RemoveFromFavoritesAsync(userId, foodPublicId);

            if (!success)
            {
                return NotFound(new { message = "Ez az étel nem található a kedvenceid között." });
            }

            return Ok(new { message = "Sikeresen eltávolítva a kedvencek közül." });
        }
    }
}
