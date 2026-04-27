using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using VizsgaRemekBackend.Data;
using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.DdSeeders
{
    public class UserSeeder
    {

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            
            await context.Database.MigrateAsync();

           

            await SeedRolesAsync(roleManager);
            await SeedUsersAsync(userManager);
            await SeedRestaurantsAsync(context);
            await SeedFoodsAsync(context);
            await SeedFoodImagesAsync(context);
            await SeedOrdersAsync(context, userManager);
            await SeedOrderItemsAsync(context);
            await SeedPaymentsAsync(context);
            await SeedReviewsAsync(context, userManager);
            await SeedFavoritesAsync(context, userManager);
        }



        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private static async Task SeedUsersAsync(UserManager<User> userManager)
        {
            var usersToSeed = new[]
            {
                new
                {
                    Email    = "admin@example.com",
                    UserName = "admin@example.com",
                    Name     = "Adminisztrátor",
                    Points   = 500,
                    Phone    = "+36301234567",
                    Role     = "Admin",
                    Password = "Admin123!"
                },
                new
                {
                    Email    = "kovacs.peter@example.com",
                    UserName = "kovacs.peter@example.com",
                    Name     = "Kovács Péter",
                    Points   = 120,
                    Phone    = "+36309876543",
                    Role     = "User",
                    Password = "User123!"
                },
                new
                {
                    Email    = "nagy.anna@example.com",
                    UserName = "nagy.anna@example.com",
                    Name     = "Nagy Anna",
                    Points   = 85,
                    Phone    = "+36201112233",
                    Role     = "User",
                    Password = "User123!"
                },
                new
                {
                    Email    = "toth.gabor@example.com",
                    UserName = "toth.gabor@example.com",
                    Name     = "Tóth Gábor",
                    Points   = 200,
                    Phone    = "+36704445566",
                    Role     = "User",
                    Password = "User123!"
                },
            };

            foreach (var u in usersToSeed)
            {
                if (await userManager.FindByEmailAsync(u.Email) is null)
                {
                    var user = new User
                    {
                        Email = u.Email,
                        UserName = u.UserName,
                        Name = u.Name,
                        Points = u.Points,
                        PhoneNumber = u.Phone,
                        EmailConfirmed = true,
                    };

                    var result = await userManager.CreateAsync(user, u.Password);
                    if (result.Succeeded)
                        await userManager.AddToRoleAsync(user, u.Role);
                }
            }
        }



        private static async Task SeedRestaurantsAsync(AppDbContext context)
        {
            if (await context.Restaurants.AnyAsync()) return;

            var restaurants = new List<Restaurant>
            {
                new Restaurant
                {
                    Name               = "Pizza Palace",
                    Address            = "Budapest, Váci út 10.",
                    Phone              = "+3614567890",
                    OpeningHours       = "10:00–22:00",
                    Category           = "Pizza",
                    RestaurantImageUrl = "https://www.millsboropizzapalace.net/images/banner-3.jpg",
                },
                new Restaurant
                {
                    Name               = "Burger Bistro",
                    Address            = "Budapest, Andrássy út 25.",
                    Phone              = "+3619876543",
                    OpeningHours       = "11:00–23:00",
                    Category           = "Burger",
                    RestaurantImageUrl = "https://static.wixstatic.com/media/dfdfac_916e876aadf24363a313cdd8644bc4f9~mv2.jpg",
                },
                new Restaurant
                {
                    Name               = "Sushi Sakura",
                    Address            = "Budapest, Rákóczi út 5.",
                    Phone              = "+3611122334",
                    OpeningHours       = "12:00–21:00",
                    Category           = "Sushi",
                    RestaurantImageUrl = "https://imageproxy.wolt.com/assets/68db87a3f18cfc617a59369c",
                },
                new Restaurant
                {
                    Name               = "Magyar Csárda",
                    Address            = "Budapest, Kossuth tér 3.",
                    Phone              = "+3615544332",
                    OpeningHours       = "09:00–21:00",
                    Category           = "Magyar",
                    RestaurantImageUrl = "https://etterem.hu/img/x/p9782n/1393246118-1490.jpg",
                },
            };

            await context.Restaurants.AddRangeAsync(restaurants);
            await context.SaveChangesAsync();
        }

      

        private static async Task SeedFoodsAsync(AppDbContext context)
        {
            if (await context.Foods.AnyAsync()) return;

            var restaurants = await context.Restaurants.ToListAsync();

            var foods = new List<Food>
            {
                // Pizza Palace
                new Food { RestaurantId = restaurants[0].Id, Name = "Margherita Pizza",   Description = "Klasszikus paradicsomos-mozzarellás pizza",    Price = 2490, Category = "Pizza" },
                new Food { RestaurantId = restaurants[0].Id, Name = "Pepperoni Pizza",    Description = "Csípős pepperoni és sajt",                       Price = 2890, Category = "Pizza" },
                new Food { RestaurantId = restaurants[0].Id, Name = "Quattro Stagioni",   Description = "Négy évszak pizza sonkával és gombával",         Price = 3190, Category = "Pizza" },
 
                // Burger Bistro
                new Food { RestaurantId = restaurants[1].Id, Name = "Classic Burger",     Description = "Marhahúspogácsa friss zöldségekkel",             Price = 1990, Category = "Burger" },
                new Food { RestaurantId = restaurants[1].Id, Name = "BBQ Bacon Burger",   Description = "BBQ szósszal és ropogós szalonnával",            Price = 2390, Category = "Burger" },
                new Food { RestaurantId = restaurants[1].Id, Name = "Veggie Burger",      Description = "Zöldségpogácsa avokádóval",                      Price = 1890, Category = "Burger" },
 
                // Sushi Sakura
                new Food { RestaurantId = restaurants[2].Id, Name = "Salmon Nigiri",      Description = "Friss lazac nigiri (2 db)",                      Price = 890,  Category = "Sushi" },
                new Food { RestaurantId = restaurants[2].Id, Name = "California Roll",    Description = "Rák, avokádó, uborka (8 db)",                   Price = 1490, Category = "Sushi" },
                new Food { RestaurantId = restaurants[2].Id, Name = "Dragon Roll",        Description = "Garnélarák, avokádó, teriyaki szósz (8 db)",    Price = 1790, Category = "Sushi" },
 
                // Magyar Csárda
                new Food { RestaurantId = restaurants[3].Id, Name = "Gulyásleves",        Description = "Hagyományos marhagulyás friss kenyérrel",       Price = 1290, Category = "Leves" },
                new Food { RestaurantId = restaurants[3].Id, Name = "Csirkepaprikás",     Description = "Tejfölös csirkepaprikás galuskával",             Price = 2190, Category = "Főétel" },
                new Food { RestaurantId = restaurants[3].Id, Name = "Lángos",             Description = "Tejföllel és sajttal",                           Price = 890,  Category = "Snack" },
            };

            await context.Foods.AddRangeAsync(foods);
            await context.SaveChangesAsync();
        }

        

        private static async Task SeedFoodImagesAsync(AppDbContext context)
        {
            if (await context.FoodImages.AnyAsync()) return;

            var foods = await context.Foods.ToListAsync();

            var imageLinks = new Dictionary<string, string>
            {
                { "Margherita Pizza", "https://safrescobaldistatic.blob.core.windows.net/media/2022/11/PIZZA-MARGHERITA.jpg" },
                { "Pepperoni Pizza", "https://assets-us-01.kc-usercontent.com/4353bced-f940-00d0-8c6e-13a0a4a7f5c2/2ac60829-5178-4a6e-80cf-6ca43d862cee/Quick-and-Easy-Pepperoni-Pizza-700x700.jpeg?w=1280&auto=format" },
                { "Quattro Stagioni", "https://acrobatica.ro/wp-content/uploads/2019/11/pizza-quattro-stagioni-cluj.png" },

                { "Classic Burger", "https://blog-content.omahasteaks.com/wp-content/uploads/2022/06/blogwp_classic-american-burger-scaled-1.jpg" },
                { "BBQ Bacon Burger", "https://blog-content.omahasteaks.com/wp-content/uploads/2022/06/blogwp_bbq-bacon-brisket-burger-with-candied-bacon-scaled-1.jpg" },
                { "Veggie Burger", "https://www.realsimple.com/thmb/z3cQCYXTyDQS9ddsqqlTVE8fnpc=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/real-simple-mushroom-black-bean-burgers-recipe-0c365277d4294e6db2daa3353d6ff605.jpg" },

                { "Salmon Nigiri", "https://cdn.tasteatlas.com/images/dishes/ef30d84ca9f04834917378c56630c6da.jpg" },
                { "California Roll", "https://upload.wikimedia.org/wikipedia/commons/9/9f/California_Sushi_%2826571101885%29.jpg" },
                { "Dragon Roll", "https://www.justonecookbook.com/wp-content/uploads/2020/06/Dragon-Roll-0286-I.jpg" },

                { "Gulyásleves", "https://production.streetkitchen-cdn.com/klasszikus-gulyasleves-2-scaled-gn_-N2.webp" },
                { "Csirkepaprikás", "https://image-api.nosalty.hu/nosalty/images/recipes/fT/EM/csirkepaprikas-javabol.jpeg?w=1200&h=920&s=4960bf27094d63b3d015b54b8e909851" },
                { "Lángos", "https://production.streetkitchen-cdn.com/klasszik-langos-scaled-yRvGSK.webp" }
            };

            var images = foods
            .Where(f => imageLinks.ContainsKey(f.Name))
            .Select(f => new FoodImage
            {
                FoodId = f.Id,
                ImageUrl = imageLinks[f.Name]
            })
            .ToList();

            await context.FoodImages.AddRangeAsync(images);
            await context.SaveChangesAsync();
        }

    

        private static async Task SeedOrdersAsync(AppDbContext context, UserManager<User> userManager)
        {
            if (await context.Orders.AnyAsync()) return;

            var users = await userManager.Users.ToListAsync();
            
            var regularUsers = users.Where(u => u.Name != "Adminisztrátor").ToList();

            var orders = new List<Order>
            {
                new Order
                {
                    UserId     = regularUsers[0].Id,
                    Status     = "completed",
                    TotalPrice = 5380,
                    OrderDate  = DateTime.UtcNow.AddDays(-7),
                },
                new Order
                {
                    UserId     = regularUsers[1].Id,
                    Status     = "completed",
                    TotalPrice = 3480,
                    OrderDate  = DateTime.UtcNow.AddDays(-3),
                },
                new Order
                {
                    UserId     = regularUsers[2].Id,
                    Status     = "pending",
                    TotalPrice = 2680,
                    OrderDate  = DateTime.UtcNow,
                },
            };

            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();
        }

  

        private static async Task SeedOrderItemsAsync(AppDbContext context)
        {
            if (await context.OrderItems.AnyAsync()) return;

            var orders = await context.Orders.ToListAsync();
            var foods = await context.Foods.Include(f => f.Restaurant).ToListAsync();

            var orderItems = new List<OrderItem>
            {
              
                new OrderItem { OrderId = orders[0].Id, FoodId = foods[0].Id, RestaurantId = foods[0].RestaurantId, Quantity = 1 },
                new OrderItem { OrderId = orders[0].Id, FoodId = foods[1].Id, RestaurantId = foods[1].RestaurantId, Quantity = 1 },
 
                
                new OrderItem { OrderId = orders[1].Id, FoodId = foods[3].Id, RestaurantId = foods[3].RestaurantId, Quantity = 2 },
 
                
                new OrderItem { OrderId = orders[2].Id, FoodId = foods[6].Id, RestaurantId = foods[6].RestaurantId, Quantity = 2 },
                new OrderItem { OrderId = orders[2].Id, FoodId = foods[7].Id, RestaurantId = foods[7].RestaurantId, Quantity = 1 },
            };

            await context.OrderItems.AddRangeAsync(orderItems);
            await context.SaveChangesAsync();
        }


        private static async Task SeedPaymentsAsync(AppDbContext context)
        {
            if (await context.Payments.AnyAsync()) return;

            var orders = await context.Orders.ToListAsync();

            var payments = new List<Payment>
            {
                new Payment
                {
                    OrderId       = orders[0].Id,
                    PaymentMethod = "card",
                    PaymentStatus = "paid",
                    PaidAt        = DateTime.UtcNow.AddDays(-7),
                },
                new Payment
                {
                    OrderId       = orders[1].Id,
                    PaymentMethod = "cash",
                    PaymentStatus = "paid",
                    PaidAt        = DateTime.UtcNow.AddDays(-3),
                },
                new Payment
                {
                    OrderId       = orders[2].Id,
                    PaymentMethod = "card",
                    PaymentStatus = "pending",
                    PaidAt        = null,
                },
            };

            await context.Payments.AddRangeAsync(payments);
            await context.SaveChangesAsync();
        }



        private static async Task SeedReviewsAsync(AppDbContext context, UserManager<User> userManager)
        {
            if (await context.Reviews.AnyAsync()) return;

            var users = await userManager.Users.ToListAsync();
            var restaurants = await context.Restaurants.ToListAsync();

            var regularUsers = users.Where(u => u.Name != "Adminisztrátor").ToList();

            var reviews = new List<Review>
            {
                new Review
                {
                    UserId       = regularUsers[0].Id,
                    RestaurantId = restaurants[0].Id,
                    Rating       = 5,
                    Comment      = "Fantasztikus pizza, nagyon gyorsan kiszállítottak!",
                    CreatedAt    = DateTime.UtcNow.AddDays(-6),
                },
                new Review
                {
                    UserId       = regularUsers[1].Id,
                    RestaurantId = restaurants[1].Id,
                    Rating       = 4,
                    Comment      = "Finom burgerek, csak a várakozási idő volt egy kicsit hosszú.",
                    CreatedAt    = DateTime.UtcNow.AddDays(-2),
                },
                new Review
                {
                    UserId       = regularUsers[2].Id,
                    RestaurantId = restaurants[2].Id,
                    Rating       = 5,
                    Comment      = "Igazi japán ízek, friss alapanyagok. Visszatérek!",
                    CreatedAt    = DateTime.UtcNow.AddDays(-1),
                },
                new Review
                {
                    UserId       = regularUsers[1].Id,
                    RestaurantId = restaurants[3].Id,
                    Rating       = 5,
                    Comment      = "Olyan volt a gulyás, mint otthon. Remek hely!",
                    CreatedAt    = DateTime.UtcNow.AddDays(-4),
                },
            };

            await context.Reviews.AddRangeAsync(reviews);
            await context.SaveChangesAsync();
        }



        private static async Task SeedFavoritesAsync(AppDbContext context, UserManager<User> userManager)
        {
            if (await context.Favorites.AnyAsync()) return;

            var users = await userManager.Users.ToListAsync();
            var foods = await context.Foods.ToListAsync();

            var regularUsers = users.Where(u => u.Name != "Adminisztrátor").ToList();

            var favorites = new List<Favorite>
            {
                new Favorite { UserId = regularUsers[0].Id, FoodId = foods[0].Id  },
                new Favorite { UserId = regularUsers[0].Id, FoodId = foods[4].Id  },
                new Favorite { UserId = regularUsers[1].Id, FoodId = foods[7].Id  },
                new Favorite { UserId = regularUsers[1].Id, FoodId = foods[10].Id },
                new Favorite { UserId = regularUsers[2].Id, FoodId = foods[2].Id  },
                new Favorite { UserId = regularUsers[2].Id, FoodId = foods[8].Id  },
            };

            await context.Favorites.AddRangeAsync(favorites);
            await context.SaveChangesAsync();
        }
    }
}
