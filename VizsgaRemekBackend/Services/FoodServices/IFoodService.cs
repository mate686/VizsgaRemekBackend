using Microsoft.AspNetCore.Mvc;
using VizsgaRemekBackend.Dtos.FoodDtos;
using VizsgaRemekBackend.Models;


namespace VizsgaRemekBackend.Services.FoodServices
{
    public interface IFoodService
    {
        Task<List<AllFoodDto>> GetAllFoodAsync();
        Task<FoodBypubId?> GetFoodByIdAsync(Guid publicid);
        Task<UpdateFoodDto?> GetUpdateFoodAsync(Guid publicid);
        Task<bool> CreateFoodAsync(Guid restaurantPublicId, CreateFoodDto dto);
        Task<bool> UpdateFoodAsync(Guid publicid, UpdateFoodDto ufood);
        Task<bool> DeleteFoodAsync(Guid publicid);
    }
}
