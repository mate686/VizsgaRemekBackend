using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VizsgaRemekBackend.Data;
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

        public async Task<string> CreateFoodAsnyc([FromBody] CreateFoodDto cfood)
        {
            try
            {
                Food food = new Food
                {
                    publicId = Guid.NewGuid(),
                    Name = cfood.Name,
                    Description = cfood.Description,
                    Price = cfood.Price,
                    Category = cfood.Category,
                    RestaurantId = cfood.RestaurantId,
                    Images = cfood.Images
                };

                _conn.Foods.Add(food);
                await _conn.SaveChangesAsync();

                return "Sikeres feltöltés";
            }
            catch (Exception err)
            {
                return $"Hiba történt: {err.Message}";
            }

        }

        public async Task<string> DeleteFoodAsnyc(Guid publicid)
        {
            try
            {
                Food? food =await _conn.Foods.FirstOrDefaultAsync(f => f.publicId == publicid);

                if (food == null)
                {
                    return "Nem található ilyen étel";
                }
                else
                {
                    _conn.Foods.Remove(food);
                    await _conn.SaveChangesAsync();
                    return "Sikeres törlés";
                }

                
            }
            catch (Exception err)
            {
                return $"Hiba történt: {err.Message}";

            }
        }

        public async Task<List<AllFoodDto>> GetAllFoodAsync()
        {
            List<AllFoodDto> allFood = await _conn.Foods
                .Select(f => new AllFoodDto
                {
                    publicId = f.publicId,
                    Name = f.Name,
                    Price = f.Price,
                    Category = f.Category,
                    Images = f.Images
                })
                .ToListAsync();

            return allFood;
        }

        public async Task<FoodBypubId?> GetFoodByIdAsnyc(Guid publicid)
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
                Images = f.Images
            })
            .FirstOrDefaultAsync();
        }

        public async Task<string> UpdateFoodAsnyc(Guid publicid, [FromBody] UpdateFoodDto ufood)
        {
            try
            {
                Food food = await _conn.Foods.FirstOrDefaultAsync(f => f.publicId == publicid);

                if (food == null)
                {
                    return "Nem található ilyen étel";
                }


                if (ufood.Name != null)
                {
                    food.Name = ufood.Name;
                }
                if (ufood.Description != null)
                {
                    food.Description = ufood.Description;
                }
                if (ufood.Price != null)
                {
                    food.Price = (decimal)ufood.Price;
                }
                if (ufood.Category != null)
                {
                    food.Category = ufood.Category;
                }
                if (ufood.Images != null) {
                    food.Images = ufood.Images;
                }

                return "Sikeres módosítás";
            }
            catch (Exception err)
            {
                return $"Hiba történt: {err.Message}";


            }
        }
    }
}
