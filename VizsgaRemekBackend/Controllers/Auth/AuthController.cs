using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using VizsgaRemekBackend.Dtos;
using VizsgaRemekBackend.Dtos.AuthDtos;
using VizsgaRemekBackend.Models;
using VizsgaRemekBackend.Services.Auth;

namespace VizsgaRemekBackend.Controllers.Auth
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _ats;

        public AuthController(IAuthService ats)
        {
            _ats = ats;
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
