using Microsoft.AspNetCore.Identity;

namespace VizsgaRemekBackend.Models
{
    public class User : IdentityUser
    {

        public string Name { get; set; } = null!;   
        public int Points { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        /*
        public int Id { get; set; }
        public Guid publicId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Role { get; set; } = "user"; 
        public int Points { get; set; } = 0;
        */
    }
}
