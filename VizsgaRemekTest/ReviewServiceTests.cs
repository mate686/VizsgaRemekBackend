using Microsoft.EntityFrameworkCore;
using VizsgaRemekBackend.Dtos.ReviewsDto;
using VizsgaRemekBackend.Models;
using VizsgaRemekBackend.Services.Reviews;

namespace VizsgaRemekTest;

public class ReviewServiceTests
{
    [Fact]
    public async Task CreateReviewAsync_Returns_Error_When_Restaurant_Does_Not_Exist()
    {
       
        using var ctx = TestDbFactory.CreateContext(nameof(CreateReviewAsync_Returns_Error_When_Restaurant_Does_Not_Exist));

        var service = new ReviewService(ctx);
        var dto = new CreateReviewDto
        {
            RestaurantPublicId = Guid.NewGuid(),
            Rating = 5,
            Comment = "Szuper hely"
        };

       
        var result = await service.CreateReviewAsync(dto, "user-1");

       
        Assert.Equal("Nincs ilyen étterem", result);
        Assert.Empty(ctx.Reviews);
    }

    [Fact]
    public async Task GetReviewsByRestaurantAsync_Returns_Reviews_In_Descending_CreatedAt_Order()
    {
        
        using var ctx = TestDbFactory.CreateContext(nameof(GetReviewsByRestaurantAsync_Returns_Reviews_In_Descending_CreatedAt_Order));

        var restaurantPublicId = Guid.NewGuid();
        var restaurant = new Restaurant
        {
            Id = 1,
            publicId = restaurantPublicId,
            Name = "Teszt Étterem",
            Address = "Debrecen, Piac utca 1.",
            Phone = "06305555555",
            OpeningHours = "10-22",
            Category = "Magyar",
            RestaurantImageUrl = "etterem.jpg"
        };

        var user1 = new User { Id = "user-1", UserName = "zoli", Name = "Zoli", Email = "zoli@example.com" };
        var user2 = new User { Id = "user-2", UserName = "eva", Name = "Éva", Email = "eva@example.com" };

        ctx.Restaurants.Add(restaurant);
        ctx.Users.AddRange(user1, user2);
        ctx.Reviews.AddRange(
            new Review
            {
                Id = 1,
                PublicId = Guid.NewGuid(),
                UserId = user1.Id,
                RestaurantId = restaurant.Id,
                Rating = 4,
                Comment = "Jó volt.",
                CreatedAt = new DateTime(2026, 4, 10, 10, 0, 0, DateTimeKind.Utc)
            },
            new Review
            {
                Id = 2,
                PublicId = Guid.NewGuid(),
                UserId = user2.Id,
                RestaurantId = restaurant.Id,
                Rating = 5,
                Comment = "Nagyon jó volt.",
                CreatedAt = new DateTime(2026, 4, 12, 10, 0, 0, DateTimeKind.Utc)
            });
        await ctx.SaveChangesAsync();

        var service = new ReviewService(ctx);

     
        var result = await service.GetReviewsByRestaurantAsync(restaurantPublicId);

        
        Assert.Equal(2, result.Count);
        Assert.Equal("Nagyon jó volt.", result[0].Comment);
        Assert.Equal("Jó volt.", result[1].Comment);
    }

    [Fact]
    public async Task DeleteReviewAsync_Returns_Forbidden_When_User_Is_Not_Owner_And_Not_Admin()
    {
        
        using var ctx = TestDbFactory.CreateContext(nameof(DeleteReviewAsync_Returns_Forbidden_When_User_Is_Not_Owner_And_Not_Admin));

        var reviewPublicId = Guid.NewGuid();
        ctx.Reviews.Add(new Review
        {
            Id = 1,
            PublicId = reviewPublicId,
            UserId = "owner-user",
            RestaurantId = 1,
            Rating = 5,
            Comment = "Nagyon jó"
        });
        await ctx.SaveChangesAsync();

        var service = new ReviewService(ctx);

      
        var result = await service.DeleteReviewAsync(reviewPublicId, "other-user", isAdmin: false);

     
        Assert.Equal(DeleteReviewResult.Forbidden, result);
        Assert.Single(ctx.Reviews);
    }

    [Fact]
    public async Task UpdateReviewAsync_Updates_Only_The_Owners_Review()
    {
        
        using var ctx = TestDbFactory.CreateContext(nameof(UpdateReviewAsync_Updates_Only_The_Owners_Review));

        var reviewPublicId = Guid.NewGuid();
        ctx.Reviews.Add(new Review
        {
            Id = 1,
            PublicId = reviewPublicId,
            UserId = "user-1",
            RestaurantId = 1,
            Rating = 3,
            Comment = "Elmegy"
        });
        await ctx.SaveChangesAsync();

        var service = new ReviewService(ctx);
        var dto = new UpdateReviewDto
        {
            PublicId = reviewPublicId,
            Rating = 5,
            Comment = "Nagyon finom volt"
        };

       
        var result = await service.UpdateReviewAsync("user-1", dto);

    
        Assert.True(result);

        var review = await ctx.Reviews.FirstAsync();
        Assert.Equal(5, review.Rating);
        Assert.Equal("Nagyon finom volt", review.Comment);
    }
}
