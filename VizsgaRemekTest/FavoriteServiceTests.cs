using Microsoft.EntityFrameworkCore;
using VizsgaRemekBackend.Models;
using VizsgaRemekBackend.Services.Favorites;

namespace VizsgaRemekTest;

public class FavoriteServiceTests
{
    [Fact]
    public async Task AddToFavoritesAsync_Adds_Favorite_When_Food_Exists_And_Not_Already_Favorite()
    {
      
        using var ctx = TestDbFactory.CreateContext(nameof(AddToFavoritesAsync_Adds_Favorite_When_Food_Exists_And_Not_Already_Favorite));

        var user = new User
        {
            Id = "user-1",
            UserName = "tesztuser",
            Name = "Teszt User",
            Email = "teszt@example.com"
        };

        var restaurant = new Restaurant
        {
            Id = 1,
            publicId = Guid.NewGuid(),
            Name = "Pizza Place",
            Address = "Kaposvár, Fő utca 1.",
            Phone = "06301234567",
            OpeningHours = "10-22",
            Category = "Pizza",
            RestaurantImageUrl = "restaurant.jpg"
        };

        var foodPublicId = Guid.NewGuid();
        var food = new Food
        {
            Id = 1,
            publicId = foodPublicId,
            RestaurantId = restaurant.Id,
            Name = "Margarita",
            Description = "Paradicsomos pizza",
            Price = 2990,
            Category = "Pizza"
        };

        ctx.Users.Add(user);
        ctx.Restaurants.Add(restaurant);
        ctx.Foods.Add(food);
        await ctx.SaveChangesAsync();

        var service = new FavoriteService(ctx);

        
        var result = await service.AddToFavoritesAsync(user.Id, foodPublicId);

     
        Assert.True(result);

        var favorite = await ctx.Favorites.FirstOrDefaultAsync(f => f.UserId == user.Id && f.FoodId == food.Id);
        Assert.NotNull(favorite);
        Assert.Equal(1, await ctx.Favorites.CountAsync());
    }

    [Fact]
    public async Task AddToFavoritesAsync_Returns_False_When_Favorite_Already_Exists()
    {
   
        using var ctx = TestDbFactory.CreateContext(nameof(AddToFavoritesAsync_Returns_False_When_Favorite_Already_Exists));

        var user = new User
        {
            Id = "user-1",
            UserName = "tesztuser",
            Name = "Teszt User",
            Email = "teszt@example.com"
        };

        var foodPublicId = Guid.NewGuid();
        var restaurant = new Restaurant
        {
            Id = 1,
            Name = "Burger House",
            Address = "Pécs, Kossuth tér 2.",
            Phone = "06301234567",
            OpeningHours = "11-23",
            Category = "Burger",
            RestaurantImageUrl = "burger.jpg"
        };

        var food = new Food
        {
            Id = 1,
            publicId = foodPublicId,
            RestaurantId = restaurant.Id,
            Name = "Sajtburger",
            Description = "Marhahúsos burger",
            Price = 3490,
            Category = "Burger"
        };

        ctx.Users.Add(user);
        ctx.Restaurants.Add(restaurant);
        ctx.Foods.Add(food);
        ctx.Favorites.Add(new Favorite { UserId = user.Id, FoodId = food.Id });
        await ctx.SaveChangesAsync();

        var service = new FavoriteService(ctx);

      
        var result = await service.AddToFavoritesAsync(user.Id, foodPublicId);

    
        Assert.False(result);
        Assert.Equal(1, await ctx.Favorites.CountAsync());
    }

    [Fact]
    public async Task GetUserFavoritesAsync_Returns_Only_The_Given_Users_Favorites()
    {
    
        using var ctx = TestDbFactory.CreateContext(nameof(GetUserFavoritesAsync_Returns_Only_The_Given_Users_Favorites));

        var restaurant = new Restaurant
        {
            Id = 1,
            Name = "Teszt Étterem",
            Address = "Szeged, Dóm tér 1.",
            Phone = "06301234567",
            OpeningHours = "09-21",
            Category = "Vegyes",
            RestaurantImageUrl = "test.jpg"
        };

        var food1 = new Food
        {
            Id = 1,
            publicId = Guid.NewGuid(),
            RestaurantId = 1,
            Name = "Gyros tál",
            Description = "Hasábbal",
            Price = 2890,
            Category = "Gyros"
        };

        var food2 = new Food
        {
            Id = 2,
            publicId = Guid.NewGuid(),
            RestaurantId = 1,
            Name = "Cézár saláta",
            Description = "Csirkével",
            Price = 2590,
            Category = "Saláta"
        };

        ctx.Restaurants.Add(restaurant);
        ctx.Foods.AddRange(food1, food2);
        ctx.Favorites.AddRange(
            new Favorite { UserId = "user-1", FoodId = food1.Id },
            new Favorite { UserId = "user-2", FoodId = food2.Id });
        await ctx.SaveChangesAsync();

        var service = new FavoriteService(ctx);

        
        var result = await service.GetUserFavoritesAsync("user-1");


        Assert.Single(result);
        Assert.Equal("Gyros tál", result[0].Name);
        Assert.DoesNotContain(result, x => x.Name == "Cézár saláta");
    }
}
