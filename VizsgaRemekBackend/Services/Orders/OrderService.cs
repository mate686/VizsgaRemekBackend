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

        private static OrderResponseDto MapOrder(Order o)
        {
            return new OrderResponseDto
            {
                PublicId = o.publicId,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                Status = o.Status,
                TotalPrice = o.TotalPrice,
                CreatedAt = o.CreatedAt,
                UpdatedAt = o.UpdatedAt,
                OrderItems = o.OrderItems.Select(oi => new OrderItemResponseDto
                {
                    Id = oi.Id,
                    FoodId = oi.FoodId,
                    Quantity = oi.Quantity,
                    RestaurantId = oi.RestaurantId,
                    Food = oi.Food == null ? null : new FoodMiniDto
                    {
                        PublicId = oi.Food.publicId,
                        Name = oi.Food.Name,
                        Price = oi.Food.Price
                    }
                }).ToList()
            };
        }

       

        public async Task<OrderResponseDto?> GetOrderByIdAsync(Guid publicid)
        {
            var order = await _conn.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Food)
                .FirstOrDefaultAsync(x => x.publicId == publicid);

            return order == null ? null : MapOrder(order);
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

            var allowedStatuses = new[] { "pending", "Paid", "Cancelled", "Completed" };

            if (!allowedStatuses.Contains(newStatus))
                return false;

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


        public async Task<List<OrderResponseDto>> GetAllOrdersAsync()
        {
            var orders = await _conn.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Food)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(MapOrder).ToList();
        }

       

        // 4. Rendelés létrehozása (CartItemDto alapján, Identity UserId-val)
        public async Task<string> CreateOrderAsync(string userId, List<CartItemDto> items)
        {
            if (items == null || !items.Any())
                return "A kosár üres.";

            if (items.Any(i => i.Quantity <= 0))
                return "Minden tétel mennyiségének nagyobbnak kell lennie 0-nál.";

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
                if (food == null)
                    continue;

                var existingItem = order.OrderItems.FirstOrDefault(oi => oi.FoodId == food.Id);

                if (existingItem != null)
                {
                    existingItem.Quantity += itemDto.Quantity;
                    existingItem.UpdatedAt = DateTime.UtcNow;
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


        public async Task<bool> UpdateItemQuantityAsync(Guid orderPublicId, Guid foodPublicId, int newQuantity, string userId, bool isAdmin)
        {
            if (newQuantity <= 0)
            {
                return await RemoveItemFromOrderAsync(orderPublicId, foodPublicId, userId, isAdmin);
            }

            var order = await _conn.Orders.FirstOrDefaultAsync(o => o.publicId == orderPublicId);
            var food = await _conn.Foods.FirstOrDefaultAsync(f => f.publicId == foodPublicId);

            if (order == null || food == null || order.Status != "pending")
                return false;

            if (!isAdmin && order.UserId != userId)
                return false;

            var orderItem = await _conn.OrderItems
                .FirstOrDefaultAsync(oi => oi.OrderId == order.Id && oi.FoodId == food.Id);

            if (orderItem == null)
                return false;

            orderItem.Quantity = newQuantity;
            orderItem.UpdatedAt = DateTime.UtcNow;

            await _conn.SaveChangesAsync();
            await RecalculateTotalAsync(order);
            return true;
        }


        public async Task<bool> RemoveItemFromOrderAsync(Guid orderPublicId, Guid foodPublicId, string userId, bool isAdmin)
        {
            var order = await _conn.Orders.FirstOrDefaultAsync(o => o.publicId == orderPublicId);
            var food = await _conn.Foods.FirstOrDefaultAsync(f => f.publicId == foodPublicId);

            if (order == null || food == null || order.Status != "pending")
                return false;

            if (!isAdmin && order.UserId != userId)
                return false;

            var orderItem = await _conn.OrderItems
                .FirstOrDefaultAsync(oi => oi.OrderId == order.Id && oi.FoodId == food.Id);

            if (orderItem == null)
                return false;

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

        public async Task<List<OrderResponseDto>> GetOrdersForUserAsync(string userId)
        {
            var orders = await _conn.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Food)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return orders.Select(MapOrder).ToList();
        }

        public async Task<int> GetUserPoints(string userId)
        {

            return await _conn.Users.Where(u => u.Id == userId).Select(u => u.Points).FirstOrDefaultAsync();
        }
    }
}
