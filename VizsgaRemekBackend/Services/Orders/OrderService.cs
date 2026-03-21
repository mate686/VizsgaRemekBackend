using Microsoft.EntityFrameworkCore;
using VizsgaRemekBackend.Data;
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

        public async Task<bool> CompleteOrderAsync(Guid publicid)
        {
            if (_conn.Orders.FirstOrDefault(x => x.publicId == publicid) != null)
            {
                _conn.Orders.FirstOrDefault(x => x.publicId == publicid).Status = "Completed";
                await _conn.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> CreateOrderAsync(Guid pubid, Guid fId)
        {
            User user = await _conn.Users.FirstOrDefaultAsync(x => Guid.Parse(x.Id) == pubid);

            Food food = await _conn.Foods.FirstOrDefaultAsync(x => x.publicId == fId);

            return true;
        }

        public async Task<bool> DeleteOrderAsync(Guid publicid)
        {

            try
            {
                _conn.Orders.Remove(_conn.Orders.FirstOrDefault(x => x.publicId == publicid));

                await _conn.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _conn.Orders.ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(Guid publicid)
        {
            return await _conn.Orders.FirstOrDefaultAsync(x => x.publicId == publicid);
        }

        public Task<bool> MadeOrderPaid(Guid publicid)
        {
            _conn.Orders.FirstOrDefault(x => x.publicId == publicid).Status = "Paid";

            return Task.FromResult(true);
        }

        public Task<bool> UpdateOrderAsync(Guid publicid, Order order)
        {
            throw new NotImplementedException();
        }
    }
}
