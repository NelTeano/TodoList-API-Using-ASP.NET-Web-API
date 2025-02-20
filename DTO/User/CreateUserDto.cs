using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web_API.DTO.User
{
    public class CreateUserDto
    {
        [Required]
        [MinLength(10, ErrorMessage = "Title cannot be less 10 characters")]
        public string username { get; set; } = string.Empty;
        [Required]
        public string password { get; set; } = string.Empty;
 
    }
}