using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VizsgaRemekBackend.Data;
using VizsgaRemekBackend.Dtos.AuthDtos;
using VizsgaRemekBackend.Models;

namespace VizsgaRemekBackend.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _conn;
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        

        public AuthService(AppDbContext conn, IConfiguration config, UserManager<User> userManager,RoleManager<IdentityRole> roleManager)
        {
            _conn = conn;
            _config = config;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            var user = new User
            {
                UserName = dto.Username,
                Email = dto.Email,
                Name = dto.Name,
                Points = dto.Points,
                PhoneNumber = dto.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);


            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                return "Sikeres regisztráció";
            }
            else
            { 
                var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));

                return errorMessage;
            }
            //return result.Succeeded;



        }

        public async Task<string?> LoginAsync(LoginDto ldto)
        {
            var user = await _userManager.FindByNameAsync(ldto.Username);

            if (user == null)
                return "Rossz felhasználo név";

            var valid = await _userManager.CheckPasswordAsync(user, ldto.Password);

            if (!valid)
                return "Rossz jelszo";

            return await GenerateJwtToken(user);
        }



        private async Task<string> GenerateJwtToken(User user)
        {

           
            var claims = new List<Claim>
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Any())
            {
                claims.Add(new Claim(ClaimTypes.Role, userRoles.First()));
            }


            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:VizsgaRemekBackend"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //Logout token elmentése egy listaba amibe a kijeltkezetteket tároljuk, (Kijelentkezés és a jwt törlése reactban történik )
        /*
        private static HashSet<string> _revokedTokens = new();

        public Task<string?> LogoutAsync(string token)
        {
            _revokedTokens.Add(token);
            return "Sikeres kijelentkezés";
        }

        public bool IsTokenRevoked(string token)
        {
            return _revokedTokens.Contains(token);
        }*/
    }
}
