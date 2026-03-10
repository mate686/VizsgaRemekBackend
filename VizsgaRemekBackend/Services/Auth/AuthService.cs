using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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

        public AuthService(AppDbContext conn, IConfiguration config, UserManager<User> userManager)
        {
            _conn = conn;
            _config = config;
            _userManager = userManager;
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

            return GenerateJwtToken(user);
        }



        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:VizsgaRemekBackend"],
                audience: _config["Jwt:"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
