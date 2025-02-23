using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web_API.Data;
using Web_API.DTO.User;
using Web_API.Mappers;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Cors;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        readonly ApplicationDBContext _context;
        public UserController(ApplicationDBContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById([FromRoute] int id) {
            var user = _context.Users.Find(id);

            if(user == null) {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserDto userModel) {
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = userModel.CreateUserRequest();
            user.password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: user.password,
                salt: System.Text.Encoding.UTF8.GetBytes("salt"),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(user);
        
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser([FromRoute] int id) {
            var user = _context.Users.Find(id);

            if(user == null) {
                return NotFound();
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            return Ok();
        }


        [HttpGet("validate")]
        [EnableCors("_myAllowSpecificOrigins")] // allow cors endpoints same as in program.cs allow origins cors value
        public IActionResult ValidateJWT()
        {
            var token = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var username = jwtToken.Claims.First(claim => claim.Type == "unique_name").Value;

            return Ok(new { username });
        }
    }
}