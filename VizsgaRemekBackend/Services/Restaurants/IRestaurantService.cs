using System;
using VizsgaRemekBackend.Dtos.RestaurantDtos;

namespace VizsgaRemekBackend.Services.Restaurants
{
    public interface IRestaurantService
    {
        public Task<List<AllRestaurantDto>> GetAllRestaurantAsync();
        public Task<GetRestaurantDto> GetRestaurantByIdAsync(Guid pubid);
        public Task<bool> CreateRestaurantAsync(CreateRestaurantDto dto);
        public Task<bool> DeleteRestaurantAsync(Guid pubid);
        public Task<bool> UpdateRestaurantAsync(Guid pubid, CreateRestaurantDto dto);
        public Task<CreateRestaurantDto> GetUpdateRestaurantAsync(Guid pubid);
    }
}
