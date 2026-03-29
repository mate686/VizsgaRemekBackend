namespace VizsgaRemekBackend.Dtos.FavoriteDtos
{
    public class FavoriteFoodDto
    {
        public Guid PublicId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public string Category { get; set; } = null!;
    }
}
