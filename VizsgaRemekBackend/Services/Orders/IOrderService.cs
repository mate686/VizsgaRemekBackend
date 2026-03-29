using VizsgaRemekBackend.Dtos.OrdeeDtos;
using VizsgaRemekBackend.Dtos.OrderDtos;
using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.Services.Orders
{
    public interface IOrderService
    {

        // Rendelés létrehozása vagy meglévő "pending" rendelés bővítése
        Task<string> CreateOrderAsync(string userId, List<CartItemDto> items);

        // Rendelés lekérése publicId alapján (tételekkel együtt)
        Task<Order?> GetOrderByIdAsync(Guid publicId);

        // Összes rendelés lekérése (például admin felületre)
        Task<List<Order>> GetAllOrdersAsync();

        // Státusz frissítése (pl: "Paid", "Shipped", "Completed", "Cancelled")
        Task<bool> UpdateOrderStatusAsync(Guid publicId, string status);

   
        Task<bool> DeleteOrderAsync(Guid publicId);

   
        Task<bool> UpdateItemQuantityAsync(Guid orderPublicId, Guid foodPublicId, int newQuantity);

        Task<bool> RemoveItemFromOrderAsync(Guid orderPublicId, Guid foodPublicId);
    }
}
