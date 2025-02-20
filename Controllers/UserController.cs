using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web_API.Data;
using Web_API.DTO.User;
using Web_API.Mappers;

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
            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(user);
        
        }
    }
}