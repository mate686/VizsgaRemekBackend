using VizsgaRemekBackend.Dtos.RestaurantDtos;

namespace VizsgaRemekBackend.Services.Restaurants
{
    public interface IRestaurantService
    {
        public Task<List<AllRestaurantDto>> GetAllRestaurantAsync();
        public Task<GetRestaurantDto> GetRestaurantByIdAsnyc(Guid pubid);
    }
}
