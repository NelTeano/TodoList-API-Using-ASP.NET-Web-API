using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web_API.DTO.Stock
{
    public class MarkCompleteTodo
    {
        public bool IsComplete { get; set; } = false;
        public DateTime? UpdatedOn { get; set; } = DateTime.Now; 
    }

    public class UpdateTodoRequestDto
    {
        [Required]
        public string Title { get; set; }  = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        public bool IsComplete { get; set; } = false;
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; } =  DateTime.Now; 

    }   
}