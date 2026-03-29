using Microsoft.EntityFrameworkCore;
using VizsgaRemekBackend.Data;
using VizsgaRemekBackend.Dtos.FavoriteDtos;
using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.Services.Favorites
{
    public class FavoriteService : IFavoriteService
    {

       
        private readonly AppDbContext _conn;

        public FavoriteService(AppDbContext conn)
        {
            _conn = conn;
        }

        public async Task<bool> AddToFavoritesAsync(string userId, Guid foodPublicId)
        {

            var food = await _conn.Foods.FirstOrDefaultAsync(f => f.publicId == foodPublicId);
            if (food == null) return false;

  
            var alreadyFavorite = await _conn.Favorites
                .AnyAsync(f => f.UserId == userId && f.FoodId == food.Id);

            if (alreadyFavorite) return false; 

            var favorite = new Favorite
            {
                UserId = userId,
                FoodId = food.Id
            };

            _conn.Favorites.Add(favorite);
            await _conn.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromFavoritesAsync(string userId, Guid foodPublicId)
        {
            var food = await _conn.Foods.FirstOrDefaultAsync(f => f.publicId == foodPublicId);
            if (food == null) return false;

            var favorite = await _conn.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.FoodId == food.Id);

            if (favorite == null) return false; 

            _conn.Favorites.Remove(favorite);
            await _conn.SaveChangesAsync();
            return true;
        }

        public async Task<List<FavoriteFoodDto>> GetUserFavoritesAsync(string userId)
        {
 
            return await _conn.Favorites
                .Where(f => f.UserId == userId)
                .Select(f => new FavoriteFoodDto
                {
                    PublicId = f.Food.publicId,
                    Name = f.Food.Name,
                    Description = f.Food.Description,
                    Price = f.Food.Price,
                    Category = f.Food.Category
                })
                .ToListAsync();
        }
    }
}

    

