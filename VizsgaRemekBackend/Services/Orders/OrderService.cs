using Microsoft.EntityFrameworkCore;
using VizsgaRemekBackend.Data;
using VizsgaRemekBackend.Dtos.OrdeeDtos;
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

        public async Task<string> CreateOrderAsync(Guid pubid, List<OrderItemDTO> orderItems)
        {
           User user = await _conn.Users.FirstOrDefaultAsync(x => Guid.Parse(x.Id) == pubid);

           Order orders = user.Orders.FirstOrDefault(x => x.Status == "pending");

            if (orders == null)
            {
                orders = new Order
                {
                    Status = "pending",
                    TotalPrice = 0,
                    User = user,
                    OrderItems = new List<OrderItem>()
                };

                foreach (var item in orders.OrderItems)
                {
                    OrderItem oi = new OrderItem
                    {
                        FoodId = item.FoodId,
                        Quantity = item.Quantity,
                        Food = item.Food,
                        OrderId = orders.Id,
                        Order = orders
                    };


                    orders.OrderItems.Add(oi);
                }

                _conn.Orders.Update(orders);
                await _conn.SaveChangesAsync();

            }
            else
            {
                foreach (var item in orders.OrderItems)
                {
                    OrderItem oi = new OrderItem
                    {
                        FoodId = item.FoodId,
                        Quantity = item.Quantity,
                        Food = item.Food,
                        OrderId = orders.Id,
                        Order = orders
                    };

                    
                    orders.OrderItems.Add(oi);
                }

                _conn.Orders.Update(orders);
                await _conn.SaveChangesAsync();


            }

            //Food food = await _conn.Foods.FirstOrDefaultAsync(x => x.publicId == fId);

            return "Sikeres";
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
