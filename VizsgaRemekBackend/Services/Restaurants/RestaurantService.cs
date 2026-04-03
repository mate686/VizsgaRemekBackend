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
            if(!(await _conn.SaveChangesAsync() > 0))
            {
                return false;
            }

            return true;
        }

   
        public async Task<bool> DeleteRestaurantAsync(Guid pubid)
        {
            var restaurant = await _conn.Restaurants.FirstOrDefaultAsync(r => r.publicId == pubid);

            if (restaurant == null)
            {
                return false; 
            }

            _conn.Restaurants.Remove(restaurant);
            await _conn.SaveChangesAsync();
            return true;
        }

        public async Task<List<AllRestaurantDto>> GetAllRestaurantAsync()
        {
            return await _conn.Restaurants.Select(r => new AllRestaurantDto
            {
                publicId = r.publicId,
                Name = r.Name,
                Address = r.Address,
                Phone = r.Phone,
                OpeningHours = r.OpeningHours,
                Category = r.Category,
                RestaurantImageUrl = r.RestaurantImageUrl
            }).ToListAsync();
        }


        public async Task<GetRestaurantDto?> GetRestaurantByIdAsync(Guid pubid)
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

    
        public async Task<CreateRestaurantDto?> GetUpdateRestaurantAsync(Guid pubid)
        {
            return await _conn.Restaurants
                .Where(r => r.publicId == pubid)
                .Select(r => new CreateRestaurantDto
                {
                    Name = r.Name,
                    Address = r.Address,
                    Phone = r.Phone,
                    OpeningHours = r.OpeningHours,
                    Category = r.Category,
                    RestaurantImageUrl = r.RestaurantImageUrl
                }).FirstOrDefaultAsync();
        }


        public async Task<bool> UpdateRestaurantAsync(Guid pubid, CreateRestaurantDto dto)
        {
            var restaurant = await _conn.Restaurants.FirstOrDefaultAsync(r => r.publicId == pubid);

            if (restaurant == null)
            {
                return false;
            }

            restaurant.Name = dto.Name;
            restaurant.Address = dto.Address;
            restaurant.Phone = dto.Phone;
            restaurant.OpeningHours = dto.OpeningHours;
            restaurant.Category = dto.Category;
            restaurant.RestaurantImageUrl = dto.RestaurantImageUrl;

       

            await _conn.SaveChangesAsync();
            return true;
        }
    }
}
