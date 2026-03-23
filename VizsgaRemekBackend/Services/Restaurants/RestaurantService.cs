using Microsoft.EntityFrameworkCore;
using VizsgaRemekBackend.Data;
using VizsgaRemekBackend.Dtos.FoodDtos;
using VizsgaRemekBackend.Dtos.FoodImagesDto;
using VizsgaRemekBackend.Dtos.RestaurantDtos;
using VizsgaRemekBackend.Dtos.UserDtos;

namespace VizsgaRemekBackend.Services.Restaurants
{
    public class RestaurantService : IRestaurantService
    {

        private readonly AppDbContext _conn;

        public RestaurantService(AppDbContext conn)
        {
            _conn = conn;
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
    }
}
