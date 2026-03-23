using System;
using VizsgaRemekBackend.Dtos.RestaurantDtos;

namespace VizsgaRemekBackend.Services.Restaurants
{
    public interface IRestaurantService
    {
        public Task<List<AllRestaurantDto>> GetAllRestaurantAsync();
        public Task<GetRestaurantDto> GetRestaurantByIdAsnyc(Guid pubid);
        public Task<bool> CreateRestaurantAsync(CreateRestaurantDto dto);
        public Task<string> DeleteRestaurantAsync(Guid pubid);
        public Task<string> UpdateRestaurantAsync(Guid pubid, CreateRestaurantDto dto);
        public Task<CreateRestaurantDto> GetUpdateRestaurantAsnyc(Guid pubid);
    }
}
