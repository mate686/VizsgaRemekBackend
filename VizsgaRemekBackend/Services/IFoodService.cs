using Microsoft.AspNetCore.Mvc;
using VizsgaRemekBackend.Dtos;
using VizsgaRemekBackend.Models;


namespace VizsgaRemekBackend.Services
{
    public interface IFoodService
    {
        public Task<List<AllFoodDto>> GetAllFoodAsync();

        public Task<FoodBypubId?> GetFoodByIdAsnyc(Guid publicid);

        public Task<string> CreateFoodAsnyc([FromBody] CreateFoodDto cfood);

        public Task<string> UpdateFoodAsnyc(Guid publicid, [FromBody] UpdateFoodDto ufood);

        public Task<string> DeleteFoodAsnyc(Guid publicid);
    }
}
