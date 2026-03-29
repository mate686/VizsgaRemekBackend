using Microsoft.EntityFrameworkCore;
using VizsgaRemekBackend.Data;
using VizsgaRemekBackend.Dtos.FoodDtos;
using VizsgaRemekBackend.Dtos.FoodImagesDto;
using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.Services.FoodServices
{
    public class FoodService : IFoodService
    {
        private readonly AppDbContext _conn;

        public FoodService(AppDbContext conn)
        {
            _conn = conn;
        }

        public async Task<bool> CreateFoodAsync(CreateFoodDto cfood)
        {
            var food = new Food
            {
                publicId = Guid.NewGuid(),
                Name = cfood.Name,
                Description = cfood.Description,
                Price = cfood.Price,
                Category = cfood.Category,
                RestaurantId = cfood.RestaurantId,
  
                Images = cfood.Images.Select(i => new FoodImage
                {
                    ImageUrl = i.ImageUrl
                }).ToList()
            };

            _conn.Foods.Add(food);
            return await _conn.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteFoodAsync(Guid publicid)
        {
            var food = await _conn.Foods.FirstOrDefaultAsync(f => f.publicId == publicid);

            if (food == null) return false;

            _conn.Foods.Remove(food);
            return await _conn.SaveChangesAsync() > 0;
        }

        public async Task<List<AllFoodDto>> GetAllFoodAsync()
        {
            return await _conn.Foods
                .Select(f => new AllFoodDto
                {
                    publicId = f.publicId,
                    Name = f.Name,
                    Price = f.Price,
                    Category = f.Category,
                    ImageUrl = f.Images.Select(i => i.ImageUrl ).ToList()
                })
                .ToListAsync();
        }

        public async Task<FoodBypubId?> GetFoodByIdAsync(Guid publicid)
        {
            return await _conn.Foods
                .Where(f => f.publicId == publicid)
                .Select(f => new FoodBypubId
                {
                    publicId = f.publicId,
                    Name = f.Name,
                    Description = f.Description,
                    Price = f.Price,
                    Category = f.Category,
                    Images = f.Images.Select(i => new FoodImageDto
                    {
                        ImageUrl = i.ImageUrl
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<UpdateFoodDto?> GetUpdateFoodAsync(Guid publicid)
        {
            return await _conn.Foods
                .Where(f => f.publicId == publicid)
                .Select(f => new UpdateFoodDto
                {
                    Name = f.Name,
                    Description = f.Description,
                    Price = f.Price,
                    Category = f.Category,
                    Images = f.Images.Select(i => new FoodImageDto
                    {
                        ImageUrl = i.ImageUrl
                    }).ToList()
                }).FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateFoodAsync(Guid publicid, UpdateFoodDto ufood)
        {

            var food = await _conn.Foods
                .Include(f => f.Images)
                .FirstOrDefaultAsync(f => f.publicId == publicid);

            if (food == null) return false;

            // Alap adatok frissítése
            food.Name = ufood.Name;
            food.Description = ufood.Description;
            food.Price = (decimal)ufood.Price;
            food.Category = ufood.Category;
            food.UpdatedAt = DateTime.UtcNow;

    
            _conn.FoodImages.RemoveRange(food.Images);

            food.Images = ufood.Images.Select(i => new FoodImage
            {
                ImageUrl = i.ImageUrl
            }).ToList();


            return await _conn.SaveChangesAsync() > 0;
        }
    }
}