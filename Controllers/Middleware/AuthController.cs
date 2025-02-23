using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web_API.DTO.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Web_API.Data;
using System.Formats.Asn1;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Web_API.Services;


namespace Web_API.Controllers.Middleware
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        readonly ApplicationDBContext _context;
        private readonly JWTService _jwtService = new JWTService("this is my custom Secret key for authentication");

        public AuthController(ApplicationDBContext context)
        {
            _context = context;
        }
    
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto userModel)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.username == userModel.username);

            if (user == null) 
                return Unauthorized(new { message = "User not found" });

            if (string.IsNullOrWhiteSpace(userModel.username) || string.IsNullOrWhiteSpace(userModel.password))
                return Unauthorized(new { message = "Invalid username or password" });

            // Hash the input password using the same PBKDF2 settings
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: userModel.password,
                salt: Encoding.UTF8.GetBytes("salt"),  // Must be the same salt as in CreateUser
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));

            if (hashedPassword != user.password) 
                return Unauthorized(new { message = "Invalid username or password" });



            var token = _jwtService.GenerateJwtToken(user.username);
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // Prevents JavaScript access (XSS protection)
                Secure = false,  // ❌ Set to false if testing on localhost (change to true in production)
                SameSite = SameSiteMode.Strict, // Prevents CSRF attacks
                Expires = DateTime.UtcNow.AddMinutes(30)
            };
            
            Response.Cookies.Append("jwt", token, cookieOptions);

            return Ok(new { token,  message = "Logged in successfully"  });
        }


        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return Ok(new { message = "Logged out successfully" });
        }



    }
}