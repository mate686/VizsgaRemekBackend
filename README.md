# VizsgaRemek Backend

## ASP.NET Core Web API

## Eszközök 

-Visual Studio 2022  
-Visual Studio 2026  
-Xampp Mysql Database  
-Insomnia (végpontok tesztelése)


## Speciális függvények 

# JWT token generálás
{
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
              audience: _config["Jwt:VizsgaRemekFrontend"],
              claims: claims,
              expires: DateTime.Now.AddHours(2),
              signingCredentials: creds
          );
          return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

#JWT adatok  
-felhasználó publikus idja (Guidja)    
-felhasználó emailcime  
-felhasználó szerepköre (User,Admin)  

## Végpontok 
