using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web_API.Data;
using Web_API.DTO.Todo;
using Web_API.Mappers;
using Web_API.Models;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public TodoController(ApplicationDBContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult GetTodos()
        {
            var todos = _context.Todos.ToList();
            return Ok(todos);
        }

        [HttpGet("{id}")]
        public IActionResult GetTodosById([FromRoute] int id) {
            var todo = _context.Todos.Find(id);

            if(todo == null) {
                return NotFound();
            }

            return Ok(todo);
        } 

        [HttpPost]
        public IActionResult CreateTodo([FromBody] CreateTodoRequestDto TodoDto) {
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var todoModel = TodoDto.CreateTodoRequestToTodo();
            _context.Todos.Add(todoModel);
            _context.SaveChanges();

            
            return CreatedAtAction(nameof(GetTodosById), new { id = todoModel.Id }, todoModel);
        }



        [HttpPatch("{id}")]
        public IActionResult MarkCompleteTodo([FromRoute] int id, [FromBody] MarkCompleteTodo todoModel) {

            var todo = _context.Todos.Find(id);

            if(todo == null) {
                return NotFound();
            }

            var updatedTodo = todoModel.TodoUpdateMarkCompleted();

            todo.IsComplete = updatedTodo.IsComplete;
            todo.UpdatedOn = updatedTodo.UpdatedOn;

            _context.SaveChanges();

            return NoContent();
        }


        [HttpPut("{id}")]
        public IActionResult EditTodoDetails([FromRoute] int id, [FromBody] UpdateTodoRequestDto todoModel){
            var todo = _context.Todos.Find(id);

            if(todo == null) {
                return NotFound();
            }

            todo.Title = todoModel.Title;
            todo.Description = todoModel.Description;   
            todo.IsComplete = todoModel.IsComplete;
            todo.CreatedOn = todoModel.CreatedOn;
            todo.UpdatedOn = todoModel.UpdatedOn;

            _context.SaveChanges();

            return Ok(todoModel.UpdateTodoRequestToTodo());
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteTodoById([FromRoute] int id) {
            var todo = _context.Todos.Find(id);

            if(todo == null) {
                return NotFound();
            }

            _context.Todos.Remove(todo);
            _context.SaveChanges();

            return Ok(todo.ToTodoDto());
        }
        
    }
}