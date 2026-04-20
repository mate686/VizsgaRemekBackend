using Microsoft.EntityFrameworkCore;
using VizsgaRemekBackend.Dtos.OrderDtos;
using VizsgaRemekBackend.Models;
using VizsgaRemekBackend.Services.Orders;

namespace VizsgaRemekTest;

public class OrderServiceTests
{
    [Fact]
    public async Task CreateOrderAsync_Returns_Error_When_Cart_Is_Empty()
    {
        
        using var ctx = TestDbFactory.CreateContext(nameof(CreateOrderAsync_Returns_Error_When_Cart_Is_Empty));
        var service = new OrderService(ctx);

        
        var result = await service.CreateOrderAsync("user-1", new List<CartItemDto>());

        
        Assert.Equal("A kosár üres.", result);
        Assert.Empty(ctx.Orders);
    }

    [Fact]
    public async Task CreateOrderAsync_Creates_Pending_Order_And_Calculates_Total()
    {
        
        using var ctx = TestDbFactory.CreateContext(nameof(CreateOrderAsync_Creates_Pending_Order_And_Calculates_Total));

        var restaurant = new Restaurant
        {
            Id = 1,
            Name = "Teszt Étterem",
            Address = "Kecskemét, Fő tér 1.",
            Phone = "0630123123",
            OpeningHours = "10-22",
            Category = "Vegyes",
            RestaurantImageUrl = "etterem.jpg"
        };

        var foodPublicId = Guid.NewGuid();
        var food = new Food
        {
            Id = 1,
            publicId = foodPublicId,
            RestaurantId = restaurant.Id,
            Name = "Bolognai",
            Description = "Sajttal",
            Price = 2500,
            Category = "Tészta"
        };

        ctx.Restaurants.Add(restaurant);
        ctx.Foods.Add(food);
        await ctx.SaveChangesAsync();

        var service = new OrderService(ctx);
        var items = new List<CartItemDto>
        {
            new() { FoodPublicId = foodPublicId, Quantity = 2 }
        };

        
        var result = await service.CreateOrderAsync("user-1", items);

        
        Assert.Equal("Sikeres", result);

        var order = await ctx.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync();
        Assert.NotNull(order);
        Assert.Equal("pending", order!.Status);
        Assert.Equal(5000, order.TotalPrice);
        Assert.Single(order.OrderItems);
        Assert.Equal(2, order.OrderItems.First().Quantity);
    }

    [Fact]
    public async Task UpdateItemQuantityAsync_Updates_Quantity_And_Recalculates_Total()
    {
        
        using var ctx = TestDbFactory.CreateContext(nameof(UpdateItemQuantityAsync_Updates_Quantity_And_Recalculates_Total));

        var restaurant = new Restaurant
        {
            Id = 1,
            Name = "Teszt Étterem",
            Address = "Eger, Dobó tér 1.",
            Phone = "0630999888",
            OpeningHours = "10-22",
            Category = "Vegyes",
            RestaurantImageUrl = "etterem.jpg"
        };

        var foodPublicId = Guid.NewGuid();
        var food = new Food
        {
            Id = 1,
            publicId = foodPublicId,
            RestaurantId = 1,
            Name = "Hamburger",
            Description = "Hasábbal",
            Price = 3000,
            Category = "Burger"
        };

        var orderPublicId = Guid.NewGuid();
        var order = new Order
        {
            Id = 1,
            publicId = orderPublicId,
            UserId = "user-1",
            Status = "pending",
            TotalPrice = 3000
        };

        var orderItem = new OrderItem
        {
            Id = 1,
            OrderId = order.Id,
            FoodId = food.Id,
            Quantity = 1,
            RestaurantId = restaurant.Id
        };

        ctx.Restaurants.Add(restaurant);
        ctx.Foods.Add(food);
        ctx.Orders.Add(order);
        ctx.OrderItems.Add(orderItem);
        await ctx.SaveChangesAsync();

        var service = new OrderService(ctx);

        
        var result = await service.UpdateItemQuantityAsync(orderPublicId, foodPublicId, 3, "user-1", isAdmin: false);

        
        Assert.True(result);

        var updatedItem = await ctx.OrderItems.FirstAsync();
        var updatedOrder = await ctx.Orders.FirstAsync();
        Assert.Equal(3, updatedItem.Quantity);
        Assert.Equal(9000, updatedOrder.TotalPrice);
    }

    [Fact]
    public async Task CheckoutOrderAsync_Uses_Points_Updates_Status_And_User_Points()
    {
        
        using var ctx = TestDbFactory.CreateContext(nameof(CheckoutOrderAsync_Uses_Points_Updates_Status_And_User_Points));

        var orderPublicId = Guid.NewGuid();
        var user = new User
        {
            Id = "user-1",
            UserName = "tesztuser",
            Name = "Teszt User",
            Email = "teszt@example.com",
            Points = 100
        };

        var order = new Order
        {
            Id = 1,
            publicId = orderPublicId,
            UserId = user.Id,
            Status = "pending",
            TotalPrice = 5000
        };

        ctx.Users.Add(user);
        ctx.Orders.Add(order);
        await ctx.SaveChangesAsync();

        var service = new OrderService(ctx);

        
        var result = await service.CheckoutOrderAsync(orderPublicId, user.Id, 100);

       
        Assert.Contains("Sikeres fizetés", result);

        var updatedOrder = await ctx.Orders.FirstAsync();
        var updatedUser = await ctx.Users.FirstAsync();

        Assert.Equal("Paid", updatedOrder.Status);
        Assert.Equal(4900, updatedOrder.TotalPrice);
        Assert.Equal(490, updatedUser.Points);
    }
}
