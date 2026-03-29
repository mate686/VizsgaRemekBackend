namespace VizsgaRemekBackend.Controllers.Orders
{
    
        public class CreateOrderDto
        {
            public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
            public string PaymentMethod { get; set; } = "card"; // card vagy cash
        }

        
    
}
