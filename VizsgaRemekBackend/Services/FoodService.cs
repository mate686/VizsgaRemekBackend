using VizsgaRemekBackend.Dtos;
using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.Services
{
    public class FoodService : IFoodService
    {
        private readonly AppDbContext _conn;

        public FoodService(AppDbContext conn)
        {
            _conn = conn;
        }

        public List<AllFoodDto> GetAllFood()
        {
            List<AllFoodDto> allFood = _conn.Foods.Select( f => new AllFoodDto
            {
                publicId = f.publicId,
                Name = f.Name,
                Price = f.Price,
                Category = f.Category
            }).ToList();


            return allFood;
        }
    }
}
