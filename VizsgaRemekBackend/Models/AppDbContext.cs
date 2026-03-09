using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VizsgaRemekBackend.Data;

namespace VizsgaRemekBackend.Models
{
    public class AppDbContext : IdentityDbContext<User>
    {

        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<FoodImage> FoodImages  { get; set; }
        public DbSet<Payment> Payments { get; set; }


        /*private readonly string _conn;

        public AppDbContext(IConfiguration conn) 
        {
            _conn = conn.GetConnectionString("DefaultConnection");            
        }*/

        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_conn, ServerVersion.AutoDetect(_conn));
        }*/

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Kulcs a Favorite táblában hiba elkerülése miatt UserId és FoodId a összetett kulcs 
            modelBuilder.Entity<Favorite>()
                .HasKey(f => new { f.UserId, f.FoodId });

            // Favorite kapcsolatok
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Food)
                .WithMany(food => food.Favorites)
                .HasForeignKey(f => f.FoodId);

            // FoodImage kapcsolat
            modelBuilder.Entity<FoodImage>()
                .HasOne(fi => fi.Food)
                .WithMany(f => f.Images)
                .HasForeignKey(fi => fi.FoodId);

            // OrderItem kapcsolat
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Food)
                .WithMany(f => f.OrderItems)
                .HasForeignKey(oi => oi.FoodId);

            // Review kapcsolat
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Restaurant)
                .WithMany(rest => rest.Reviews)
                .HasForeignKey(r => r.RestaurantId);

            // Payment kapcsolat
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId);
        }
    }
}
