namespace VizsgaRemekBackend.Models
{
    public class Favorite
    {
        public string UserId { get; set; }
        public int FoodId { get; set; }

        public User User { get; set; } = null!;
        public Food Food { get; set; } = null!;
    }

}
