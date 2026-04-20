using VizsgaRemekBackend.Dtos.RestaurantDtos;
using VizsgaRemekBackend.Models;
using VizsgaRemekBackend.Services.Restaurants;

namespace VizsgaRemekTest;

public class RestaurantServiceTests
{
    [Fact]
    public async Task GetAllRestaurantAsync_Returns_All_Restaurants()
    {
        
        using var ctx = TestDbFactory.CreateContext(nameof(GetAllRestaurantAsync_Returns_All_Restaurants));

        ctx.Restaurants.AddRange(
            new Restaurant
            {
                Id = 1,
                Name = "Alfa Bisztró",
                Address = "Budapest, Alfa utca 1.",
                Phone = "0611111111",
                OpeningHours = "10-22",
                Category = "Bisztró",
                RestaurantImageUrl = "alfa.jpg"
            },
            new Restaurant
            {
                Id = 2,
                Name = "Béta Grill",
                Address = "Pécs, Béta utca 2.",
                Phone = "0622222222",
                OpeningHours = "11-23",
                Category = "Grill",
                RestaurantImageUrl = "beta.jpg"
            });

        await ctx.SaveChangesAsync();

        var service = new RestaurantService(ctx);

        
        var result = await service.GetAllRestaurantAsync();

        
        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Name == "Alfa Bisztró");
        Assert.Contains(result, x => x.Name == "Béta Grill");
    }

    [Fact]
    public async Task GetRestaurantByIdAsync_Returns_Restaurant_With_Foods_And_Reviews()
    {
        
        using var ctx = TestDbFactory.CreateContext(nameof(GetRestaurantByIdAsync_Returns_Restaurant_With_Foods_And_Reviews));

        var restaurantPublicId = Guid.NewGuid();
        var reviewPublicId = Guid.NewGuid();

        var user = new User
        {
            Id = "user-1",
            UserName = "anna",
            Name = "Anna",
            Email = "anna@example.com",
            Points = 25
        };

        var restaurant = new Restaurant
        {
            Id = 1,
            publicId = restaurantPublicId,
            Name = "Teszt Étterem",
            Address = "Sopron, Várkerület 1.",
            Phone = "06301111111",
            OpeningHours = "10-22",
            Category = "Magyar",
            RestaurantImageUrl = "etterem.jpg"
        };

        var food = new Food
        {
            Id = 1,
            publicId = Guid.NewGuid(),
            RestaurantId = 1,
            Name = "Gulyásleves",
            Description = "Házi kenyérrel",
            Price = 2190,
            Category = "Leves",
            Images = new List<FoodImage>
            {
                new() { Id = 1, ImageUrl = "gulyas.jpg" }
            }
        };

        var review = new Review
        {
            Id = 1,
            PublicId = reviewPublicId,
            UserId = user.Id,
            RestaurantId = restaurant.Id,
            Rating = 5,
            Comment = "Nagyon finom volt."
        };

        ctx.Users.Add(user);
        ctx.Restaurants.Add(restaurant);
        ctx.Foods.Add(food);
        ctx.Reviews.Add(review);
        await ctx.SaveChangesAsync();

        var service = new RestaurantService(ctx);

       
        var result = await service.GetRestaurantByIdAsync(restaurantPublicId);

        
        Assert.NotNull(result);
        Assert.Equal("Teszt Étterem", result!.Name);
        Assert.Single(result.Foods);
        Assert.Single(result.Reviews);
        Assert.Equal("Gulyásleves", result.Foods.First().Name);
        Assert.Equal("Nagyon finom volt.", result.Reviews.First().Comment);
        Assert.Equal("anna", result.Reviews.First().User.UserName);
    }

    [Fact]
    public async Task UpdateRestaurantAsync_Updates_Restaurant_Data()
    {
       
        using var ctx = TestDbFactory.CreateContext(nameof(UpdateRestaurantAsync_Updates_Restaurant_Data));

        var publicId = Guid.NewGuid();
        ctx.Restaurants.Add(new Restaurant
        {
            Id = 1,
            publicId = publicId,
            Name = "Régi Név",
            Address = "Régi cím",
            Phone = "061234567",
            OpeningHours = "08-18",
            Category = "Gyorsétterem",
            RestaurantImageUrl = "old.jpg"
        });
        await ctx.SaveChangesAsync();

        var service = new RestaurantService(ctx);
        var dto = new CreateRestaurantDto
        {
            Name = "Új Név",
            Address = "Új cím",
            Phone = "069876543",
            OpeningHours = "09-21",
            Category = "Bisztró",
            RestaurantImageUrl = "new.jpg"
        };

   
        var result = await service.UpdateRestaurantAsync(publicId, dto);

   
        Assert.True(result);

        var updatedRestaurant = await ctx.Restaurants.FindAsync(1);
        Assert.NotNull(updatedRestaurant);
        Assert.Equal("Új Név", updatedRestaurant!.Name);
        Assert.Equal("Új cím", updatedRestaurant.Address);
        Assert.Equal("new.jpg", updatedRestaurant.RestaurantImageUrl);
    }
}
