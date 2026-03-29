namespace VizsgaRemekBackend.Dtos.OrderDtos
{
    
        public class CreateOrderDto
        {
            public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
            public string PaymentMethod { get; set; } = "card"; 
        }

        
    
}
