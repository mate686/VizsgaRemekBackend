using VizsgaRemekBackend.Dtos.OrdeeDtos;
using VizsgaRemekBackend.Dtos.OrderDtos;
using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.Services.Orders
{
    public interface IOrderService
    {

        Task<string> CreateOrderAsync(string userId, List<CartItemDto> items);

        Task<Order?> GetOrderByIdAsync(Guid publicId);

        Task<List<Order>> GetAllOrdersAsync();

        Task<bool> UpdateOrderStatusAsync(Guid publicId, string status);

        Task<bool> DeleteOrderAsync(Guid publicId);

        Task<string> CheckoutOrderAsync(Guid orderPublicId, string userId, int pointsToUse);


        Task<List<Order>> GetOrdersForUserAsync(string userId);

        Task<bool> UpdateItemQuantityAsync(Guid orderPublicId, Guid foodPublicId, int newQuantity, string userId, bool isAdmin);

        Task<bool> RemoveItemFromOrderAsync(Guid orderPublicId, Guid foodPublicId, string userId, bool isAdmin);
    }
}
