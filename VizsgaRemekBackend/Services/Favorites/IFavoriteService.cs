using VizsgaRemekBackend.Dtos.FavoriteDtos;

namespace VizsgaRemekBackend.Services.Favorites
{
    public interface IFavoriteService
    {
        Task<bool> AddToFavoritesAsync(string userId, Guid foodPublicId);
        Task<bool> RemoveFromFavoritesAsync(string userId, Guid foodPublicId);
        Task<List<FavoriteFoodDto>> GetUserFavoritesAsync(string userId);
    }
}
