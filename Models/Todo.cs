using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web_API.Models
{
    public class Todo
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsComplete { get; set; } = false;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? UpdatedOn { get; set; } = null;

    }
}