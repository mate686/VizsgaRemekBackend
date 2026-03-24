using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using VizsgaRemekBackend.Data;
using VizsgaRemekBackend.Dtos.FoodDtos;
using VizsgaRemekBackend.Dtos.FoodImagesDto;
using VizsgaRemekBackend.Dtos.RestaurantDtos;
using VizsgaRemekBackend.Dtos.UserDtos;
using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.Services.Restaurants
{
    public class RestaurantService : IRestaurantService
    {

        private readonly AppDbContext _conn;

        public RestaurantService(AppDbContext conn)
        {
            _conn = conn;
        }

        public async Task<bool> CreateRestaurantAsync(CreateRestaurantDto dto)
        {
            var restaurant = new Restaurant
            {
                Name = dto.Name,
                Address = dto.Address,
                Phone = dto.Phone,
                OpeningHours = dto.OpeningHours,
                Category = dto.Category,
                RestaurantImageUrl = dto.RestaurantImageUrl
            };
            _conn.Restaurants.Add(restaurant);
            await _conn.SaveChangesAsync();

            return true;

        }

        public async Task<string> DeleteRestaurantAsync(Guid pubid)
        {
            Restaurant? restaurant = _conn.Restaurants.FirstOrDefault(r => r.publicId == pubid);

            if (restaurant == null) {
                return "Nem található ilyen étterem";
            }
            else
            {
                _conn.Restaurants.Remove(restaurant);
                await _conn.SaveChangesAsync();
                return "Sikeres törlés";
            }

        }

        public async Task<List<AllRestaurantDto>> GetAllRestaurantAsync()
        {
            List<AllRestaurantDto> allr = await _conn.Restaurants.Select(r => new AllRestaurantDto
            {
                publicId = r.publicId,
                Name = r.Name,
                Address = r.Address,
                Phone = r.Phone,
                OpeningHours = r.OpeningHours,
                Category = r.Category,
                RestaurantImageUrl = r.RestaurantImageUrl
            }).ToListAsync();

            return allr;
        }

        public async Task<GetRestaurantDto?> GetRestaurantByIdAsnyc(Guid pubid)
        {
            return await _conn.Restaurants
                .Where(r => r.publicId == pubid)
                .Select(r => new GetRestaurantDto
                {
                    publicId = r.publicId,
                    Name = r.Name,
                    Address = r.Address,
                    Phone = r.Phone,
                    OpeningHours = r.OpeningHours,
                    Category = r.Category,
                    RestaurantImageUrl = r.RestaurantImageUrl,

                    Foods = r.Foods.Select(f => new FoodBypubId
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
                    }).ToList(),

                    Reviews = r.Reviews.Select(rev => new RestaurantReviewsDto
                    {
                        PublicId = rev.PublicId,
                        Rating = rev.Rating,
                        Comment = rev.Comment,
                        User = new UserDto
                        {
                            Id = rev.User.Id,
                            Name = rev.User.Name,
                            UserName = rev.User.UserName,
                            Email = rev.User.Email,
                            Points = rev.User.Points
                        }
                    }).ToList()
                }).FirstOrDefaultAsync();
        }

        public async Task<CreateRestaurantDto?> GetUpdateRestaurantAsnyc(Guid pubid)
        {
            return await _conn.Restaurants.Where(r => r.publicId == pubid).Select(r => new CreateRestaurantDto
            {
                Name = r.Name,
                Address = r.Address,
                Phone = r.Phone,
                OpeningHours = r.OpeningHours,
                Category = r.Category,
                RestaurantImageUrl = r.RestaurantImageUrl
            }).FirstOrDefaultAsync();
        }

        public async Task<string> UpdateRestaurantAsync(Guid pubid, CreateRestaurantDto dto)
        {
            Restaurant? restaurant = _conn.Restaurants.FirstOrDefault(r => r.publicId == pubid);

            if (restaurant == null)
            {
                return "Nem található ilyen étterem";
            }
            else
            {
                restaurant.Name = dto.Name;
                restaurant.Address = dto.Address;
                restaurant.Phone = dto.Phone;
                restaurant.OpeningHours = dto.OpeningHours;
                restaurant.Category = dto.Category;
                restaurant.RestaurantImageUrl = dto.RestaurantImageUrl;
                _conn.Restaurants.Update(restaurant);
                await _conn.SaveChangesAsync();
                return "Sikeres módosítás";
            }
        }
    }
}
