using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Exchangeapi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        
        private static List<User> users = new List<User>
        {
            new User { Username = "user1", Password = "password1" },
            new User { Username = "user2", Password = "password2" }
        };

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin model)
        {
            
            var user = users.Find(u => u.Username == model.Username && u.Password == model.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Username or password is incorrect" });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("ThisIsASecretKeyWith32Characters12345");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Username) }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }
    }

    public class UserLogin
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class User
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
