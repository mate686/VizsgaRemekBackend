using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<string> CreateFoodAsnyc(CreateFoodDto cfood)
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
                Images = f.Images.Select(i => new FoodImageDto
                {
                    ImageUrl = i.ImageUrl
                }).ToList()
            })
            .FirstOrDefaultAsync();
        }

        public async Task<UpdateFoodDto?> GetUpdateFoodAsnyc(Guid publicid)
        {
            return await _conn.Foods.Where(f => f.publicId == publicid)
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

        public async Task<string> UpdateFoodAsnyc(Guid publicid,UpdateFoodDto ufood)
        {
            try
            {
                Food food = await _conn.Foods.FirstOrDefaultAsync(f => f.publicId == publicid);

                if (food == null)
                {
                    return "Nem található ilyen étel";
                }

                food.Name = ufood.Name;
                food.Description = ufood.Description;
                food.Price = (decimal)ufood.Price;
                food.Category = ufood.Category;
                food.Images = food.Images.Select(i => new FoodImage
                {
                    ImageUrl = i.ImageUrl
                }).ToList();
                

                return "Sikeres módosítás";
            }
            catch (Exception err)
            {
                return $"Hiba történt: {err.Message}";


            }
        }
    }
}
