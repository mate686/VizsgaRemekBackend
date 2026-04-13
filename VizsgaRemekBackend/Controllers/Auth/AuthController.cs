using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VizsgaRemekBackend.Dtos;
using VizsgaRemekBackend.Dtos.AuthDtos;
using VizsgaRemekBackend.Models;
using VizsgaRemekBackend.Services.Auth;
using VizsgaRemekBackend.Services.Emails;

namespace VizsgaRemekBackend.Controllers.Auth
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _ats;
        private readonly IEmailService _ems;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _config;

        public AuthController(IAuthService ats,IEmailService ems, UserManager<User> userManager, IConfiguration config)
        {
            _ats = ats;
            _ems = ems;
            _userManager = userManager;
            _config = config;
        
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto rdto)
        {

            var result = await _ats.RegisterAsync(rdto);


            if (result == "Sikeres regisztráció")
            {
               return Created();
            }
            else
            {
                return BadRequest(result);
            }

            /*if (result)
            {
                return Created();
            }
            else
            {
                return BadRequest("Sikertelen regisztráció");
            }*/

        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto ldto)
        {

            var token = await _ats.LoginAsync(ldto);

            if (token == "Rossz felhasználo név")
            {
                return BadRequest("Hibás felhasználo nev");
            }
            else if (token == "Rossz jelszo")
            {
                return BadRequest("Hibás jelszó");
            }
            else
            {
                return Ok(new {token});
            }
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return Ok("Ha létezik ez az e-mail cím a rendszerben, elküldtük a visszaállítási linket.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);

            var frontendUrl = _config["FrontendUrl"];

            var resetLink = $"{frontendUrl}/reset-password?email={user.Email}&token={encodedToken}";


            var emailBody = $@"
            <h2>Jelszó visszaállítása</h2>
            <p>Kedves {user.UserName}!</p>
            <p>Kaptunk egy kérést a jelszavad visszaállítására. Kattints az alábbi linkre az új jelszó megadásához:</p>
            <p><a href='{resetLink}' style='padding: 10px 20px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px;'>Jelszó visszaállítása</a></p>
            <p>Ha nem te kérted a visszaállítást, kérjük, hagyd figyelmen kívül ezt az üzenetet.</p>
            <br>
            <p>Üdvözlettel,<br>A VizsgaRemek Csapata</p>";

  
            await _ems.SendEmailAsync(user.Email, "Jelszó visszaállítása - VizsgaRemek", emailBody);

            return Ok("Ha létezik ez az e-mail cím a rendszerben, elküldtük a visszaállítási linket.");
        }


        [HttpPost("reset-password")]
        [AllowAnonymous] 
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) return BadRequest("Hiba történt a jelszó visszaállítása során.");

  
            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new { message = "A jelszavad sikeresen megváltozott!" });
            }

            return BadRequest(new { errors = result.Errors });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var profile = await _ats.GetProfileAsync(userId!);
            if (profile == null) return NotFound("Felhasználó nem található.");

            return Ok(profile);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var (profile, error) = await _ats.UpdateProfileAsync(userId!, dto);
            if (error != null) return BadRequest(error);

            return Ok(profile);
        }

        //Kijelentkezés reactban történik a token törlése a localstorage-ból
        /*
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _ats.LogoutAsync();
            return Ok("Sikeres kijelentkezés");
        }*/


    }
}
