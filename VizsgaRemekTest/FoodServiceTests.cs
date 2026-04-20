using Microsoft.EntityFrameworkCore;
using VizsgaRemekBackend.Dtos.FoodDtos;
using VizsgaRemekBackend.Dtos.FoodImagesDto;
using VizsgaRemekBackend.Models;
using VizsgaRemekBackend.Services.FoodServices;

namespace VizsgaRemekTest;

public class FoodServiceTests
{
    [Fact]
    public async Task CreateFoodAsync_Creates_Food_With_Images_When_Restaurant_Exists()
    { 

        using var ctx = TestDbFactory.CreateContext(nameof(CreateFoodAsync_Creates_Food_With_Images_When_Restaurant_Exists));

        var restaurantPublicId = Guid.NewGuid();
        ctx.Restaurants.Add(new Restaurant
        {
            Id = 1,
            publicId = restaurantPublicId,
            Name = "Olasz Konyha",
            Address = "Budapest, Fő utca 10.",
            Phone = "06301112233",
            OpeningHours = "10-22",
            Category = "Olasz",
            RestaurantImageUrl = "olasz.jpg"
        });
        await ctx.SaveChangesAsync();

        var service = new FoodService(ctx);
        var dto = new CreateFoodDto
        {
            Name = "Carbonara",
            Description = "Tejszínes tészta",
            Price = 3990,
            Category = "Tészta",
            Images = new List<FoodImageDto>
            {
                new() { ImageUrl = "carbonara-1.jpg" },
                new() { ImageUrl = "carbonara-2.jpg" }
            }
        };

    
        var result = await service.CreateFoodAsync(restaurantPublicId, dto);

     
        Assert.True(result);

        var createdFood = await ctx.Foods.Include(f => f.Images).FirstOrDefaultAsync();
        Assert.NotNull(createdFood);
        Assert.Equal("Carbonara", createdFood!.Name);
        Assert.Equal(2, createdFood.Images.Count);
        Assert.All(createdFood.Images, image => Assert.False(string.IsNullOrWhiteSpace(image.ImageUrl)));
    }

    [Fact]
    public async Task GetAllFoodAsync_Returns_Foods_With_ImageUrls()
    {
       
        using var ctx = TestDbFactory.CreateContext(nameof(GetAllFoodAsync_Returns_Foods_With_ImageUrls));

        var restaurant = new Restaurant
        {
            Id = 1,
            Name = "Teszt Étterem",
            Address = "Győr, Fő tér 1.",
            Phone = "06301112233",
            OpeningHours = "09-20",
            Category = "Vegyes",
            RestaurantImageUrl = "etterem.jpg"
        };

        var food = new Food
        {
            Id = 1,
            publicId = Guid.NewGuid(),
            RestaurantId = 1,
            Name = "Rántott sajt",
            Description = "Tartárral",
            Price = 2790,
            Category = "Főétel",
            Images = new List<FoodImage>
            {
                new() { Id = 1, ImageUrl = "rantottsajt-1.jpg" },
                new() { Id = 2, ImageUrl = "rantottsajt-2.jpg" }
            }
        };

        ctx.Restaurants.Add(restaurant);
        ctx.Foods.Add(food);
        await ctx.SaveChangesAsync();

        var service = new FoodService(ctx);

   
        var result = await service.GetAllFoodAsync();

   
        Assert.Single(result);
        Assert.Equal("Rántott sajt", result[0].Name);
        Assert.Equal(2, result[0].ImageUrl.Count);
        Assert.Contains("rantottsajt-1.jpg", result[0].ImageUrl);
    }

    [Fact]
    public async Task UpdateFoodAsync_Updates_Fields_And_Replaces_Images()
    {
   
        using var ctx = TestDbFactory.CreateContext(nameof(UpdateFoodAsync_Updates_Fields_And_Replaces_Images));

        var restaurant = new Restaurant
        {
            Id = 1,
            Name = "Teszt Étterem",
            Address = "Miskolc, Fő utca 5.",
            Phone = "06301112233",
            OpeningHours = "10-21",
            Category = "Vegyes",
            RestaurantImageUrl = "etterem.jpg"
        };

        var foodPublicId = Guid.NewGuid();
        var food = new Food
        {
            Id = 1,
            publicId = foodPublicId,
            RestaurantId = 1,
            Name = "Palacsinta",
            Description = "Lekváros",
            Price = 990,
            Category = "Desszert",
            Images = new List<FoodImage>
            {
                new() { Id = 1, ImageUrl = "old.jpg" }
            }
        };

        ctx.Restaurants.Add(restaurant);
        ctx.Foods.Add(food);
        await ctx.SaveChangesAsync();

        var service = new FoodService(ctx);
        var dto = new UpdateFoodDto
        {
            Name = "Túrós palacsinta",
            Description = "Vaníliás öntettel",
            Price = 1490,
            Category = "Desszert",
            Images = new List<FoodImageDto>
            {
                new() { ImageUrl = "new-1.jpg" },
                new() { ImageUrl = "new-2.jpg" }
            }
        };

        var result = await service.UpdateFoodAsync(foodPublicId, dto);


        Assert.True(result);

        var updatedFood = await ctx.Foods.Include(f => f.Images).FirstAsync(f => f.publicId == foodPublicId);
        Assert.Equal("Túrós palacsinta", updatedFood.Name);
        Assert.Equal("Vaníliás öntettel", updatedFood.Description);
        Assert.Equal(1490, updatedFood.Price);
        Assert.Equal(2, updatedFood.Images.Count);
        Assert.DoesNotContain(updatedFood.Images, x => x.ImageUrl == "old.jpg");
    }
}
