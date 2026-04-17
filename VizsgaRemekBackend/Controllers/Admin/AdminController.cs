using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VizsgaRemekBackend.Data;
using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.Controllers.Admin
{
    /// <summary>
    /// Admin-only végpontok a WPF dashboard számára.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _conn;
        private readonly UserManager<User> _userManager;

        public AdminController(AppDbContext conn, UserManager<User> userManager)
        {
            _conn = conn;
            _userManager = userManager;
        }

        // GET /api/admin/users
        // Visszaadja az összes felhasználót szerepkörrel együtt
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();

            var result = new List<object>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new
                {
                    user.Id,
                    user.Name,
                    user.UserName,
                    user.Email,
                    user.Points,
                    Role = roles.FirstOrDefault() ?? "User"
                });
            }

            return Ok(result);
        }

        // GET /api/admin/reviews
        // Visszaadja az összes értékelést étterem névvel és felhasználónévvel
        [HttpGet("reviews")]
        public async Task<IActionResult> GetAllReviews()
        {
            var reviews = await _conn.Reviews
                .Include(r => r.User)
                .Include(r => r.Restaurant)
                .Select(r => new
                {
                    //r.PublicId,
                    RestaurantName = r.Restaurant.Name,
                    UserName = r.User.UserName,
                    r.Rating,
                    r.Comment,
                    r.CreatedAt
                })
                .ToListAsync();

            return Ok(reviews);
        }

        // GET /api/admin/stats
        // Összesített statisztikák egy hívásban (opcionális, gyorsabb dashboard töltés)
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var restaurantCount = await _conn.Restaurants.CountAsync();
            var orderCount = await _conn.Orders.CountAsync();
            var userCount = await _userManager.Users.CountAsync();

            var totalRevenue = await _conn.Orders
                .Where(o => o.Status == "Paid")
                .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;

            var pendingOrders = await _conn.Orders.CountAsync(o => o.Status == "pending");
            var paidOrders = await _conn.Orders.CountAsync(o => o.Status == "Paid");

            var topUsers = await _conn.Users
                .OrderByDescending(u => u.Points)
                .Take(10)
                .Select(u => new { u.UserName, u.Name, u.Points })
                .ToListAsync();

            var foodsByCategory = await _conn.Foods
                .GroupBy(f => f.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToListAsync();

            var ratingDistribution = await _conn.Reviews
                .GroupBy(r => r.Rating)
                .Select(g => new { Rating = g.Key, Count = g.Count() })
                .ToListAsync();

            return Ok(new
            {
                restaurantCount,
                orderCount,
                userCount,
                totalRevenue,
                pendingOrders,
                paidOrders,
                topUsers,
                foodsByCategory,
                ratingDistribution
            });
        }
    }
}