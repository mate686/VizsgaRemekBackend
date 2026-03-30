using Microsoft.EntityFrameworkCore;
using VizsgaRemekBackend.Data;
using VizsgaRemekBackend.Dtos.OrdeeDtos;
using VizsgaRemekBackend.Dtos.OrderDtos;
using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.Services.Orders
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _conn;

        public OrderService(AppDbContext conn)
        {
            _conn = conn;
        }

        private async Task RecalculateTotalAsync(Order order)
        {
            var items = await _conn.OrderItems
                .Include(oi => oi.Food)
                .Where(oi => oi.OrderId == order.Id)
                .ToListAsync();

            order.TotalPrice = items.Sum(oi => oi.Quantity * oi.Food.Price);
            order.UpdatedAt = DateTime.UtcNow;
            await _conn.SaveChangesAsync();
        }

      
        public async Task<bool> UpdateOrderStatusAsync(Guid publicid, string newStatus)
        {
            var order = await _conn.Orders.FirstOrDefaultAsync(x => x.publicId == publicid);
            if (order == null) return false;

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;

            await _conn.SaveChangesAsync();
            return true;
        }

       
        public async Task<bool> DeleteOrderAsync(Guid publicid)
        {
            var order = await _conn.Orders.FirstOrDefaultAsync(x => x.publicId == publicid);
            if (order == null) return false;

            _conn.Orders.Remove(order);
            await _conn.SaveChangesAsync();
            return true;
        }

       
        public async Task<List<Order>> GetAllOrdersAsync() => await _conn.Orders.ToListAsync();

        public async Task<Order?> GetOrderByIdAsync(Guid publicid) =>
            await _conn.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Food) 
                .FirstOrDefaultAsync(x => x.publicId == publicid);

        // 4. Rendelés létrehozása (CartItemDto alapján, Identity UserId-val)
        public async Task<string> CreateOrderAsync(string userId, List<CartItemDto> items)
        {
            if (items == null || !items.Any()) return "A kosár üres.";

            var order = await _conn.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.UserId == userId && o.Status == "pending");

            if (order == null)
            {
                order = new Order
                {
                    UserId = userId,
                    Status = "pending",
                    TotalPrice = 0,
                    OrderItems = new List<OrderItem>()
                };
                _conn.Orders.Add(order);
                await _conn.SaveChangesAsync(); 
            }

            foreach (var itemDto in items)
            {
                var food = await _conn.Foods.FirstOrDefaultAsync(f => f.publicId == itemDto.FoodPublicId);
                if (food == null) continue;

                var existingItem = order.OrderItems.FirstOrDefault(oi => oi.FoodId == food.Id);
                if (existingItem != null)
                {
                    existingItem.Quantity += itemDto.Quantity;
                }
                else
                {
                    _conn.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.Id,
                        FoodId = food.Id,
                        Quantity = itemDto.Quantity,
                        RestaurantId = food.RestaurantId
                    });
                }
            }

            await _conn.SaveChangesAsync();
            await RecalculateTotalAsync(order); 
            return "Sikeres";
        }

      
        public async Task<bool> UpdateItemQuantityAsync(Guid orderPublicId, Guid foodPublicId, int newQuantity)
        {
            if (newQuantity <= 0)
            {
                return await RemoveItemFromOrderAsync(orderPublicId, foodPublicId);
            }

            var order = await _conn.Orders.FirstOrDefaultAsync(o => o.publicId == orderPublicId);
            var food = await _conn.Foods.FirstOrDefaultAsync(f => f.publicId == foodPublicId);

            if (order == null || food == null || order.Status != "pending") return false;

            var orderItem = await _conn.OrderItems
                .FirstOrDefaultAsync(oi => oi.OrderId == order.Id && oi.FoodId == food.Id);

            if (orderItem == null) return false;

            orderItem.Quantity = newQuantity;
            orderItem.UpdatedAt = DateTime.UtcNow;

            await _conn.SaveChangesAsync();
            await RecalculateTotalAsync(order);
            return true;
        }

    
        public async Task<bool> RemoveItemFromOrderAsync(Guid orderPublicId, Guid foodPublicId)
        {
            var order = await _conn.Orders.FirstOrDefaultAsync(o => o.publicId == orderPublicId);
            var food = await _conn.Foods.FirstOrDefaultAsync(f => f.publicId == foodPublicId);

            if (order == null || food == null || order.Status != "pending") return false;

            var orderItem = await _conn.OrderItems
                .FirstOrDefaultAsync(oi => oi.OrderId == order.Id && oi.FoodId == food.Id);

            if (orderItem == null) return false;

            _conn.OrderItems.Remove(orderItem);
            await _conn.SaveChangesAsync();
            await RecalculateTotalAsync(order);
            return true;
        }

        public async Task<string> CheckoutOrderAsync(Guid orderPublicId, string userId, int pointsToUse)
        {

            var order = await _conn.Orders.FirstOrDefaultAsync(o => o.publicId == orderPublicId && o.UserId == userId);
            if (order == null) return "Rendelés nem található, vagy nem a te rendelésed.";

            if (order.Status != "pending") return "Ezt a rendelést már véglegesítették.";

   
            var user = await _conn.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return "Felhasználó nem található.";


            if (pointsToUse < 0) return "A felhasználandó pontok száma nem lehet negatív.";
            if (user.Points < pointsToUse) return "Nincs elég pontod ehhez a kedvezményhez.";

     
            if (order.TotalPrice < pointsToUse) return "Több pontot próbálsz beváltani, mint a rendelés végösszege.";

            user.Points -= pointsToUse;
            var finalPrice = order.TotalPrice - pointsToUse;
            order.TotalPrice = finalPrice;

  
            int earnedPoints = (int)(finalPrice * 0.1m); 
            user.Points += earnedPoints;

 
            order.Status = "Paid";
            order.UpdatedAt = DateTime.UtcNow;

            await _conn.SaveChangesAsync();

            return $"Sikeres fizetés! Levonva: {pointsToUse} pont. Új egyenleged: {user.Points} pont (Kaptál {earnedPoints} új pontot).";
        }
    }
}
