using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web_API.DTO.Stock
{
    public class CreateTodoRequestDto
    {
        [Required]
        [MaxLength(10, ErrorMessage = "Title cannot be over 10 over characters")]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        // public bool IsComplete { get; set; } = false;
        // public DateTime CreatedOn { get; set; } = DateTime.Now;
        // public DateTime? UpdatedOn { get; set; } = null; 

    }
}