using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.Services.Orders
{
    public interface IOrderService
    {

        public Task<List<Order>> GetAllOrdersAsync();

        public Task<Order?> GetOrderByIdAsync(Guid publicid);

        public Task<bool> CreateOrderAsync(Guid pubid, Guid fId);

        public Task<bool> UpdateOrderAsync(Guid publicid, Order order);

        public Task<bool> DeleteOrderAsync(Guid publicid);

        public Task<bool> CompleteOrderAsync(Guid publicid);

        public Task<bool> MadeOrderPaid(Guid publicid);
    }
}
