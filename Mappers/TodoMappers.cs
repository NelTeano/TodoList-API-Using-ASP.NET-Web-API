using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_API.Models;
using Web_API.DTO.Stock;

namespace Web_API.Mappers
{
    public static class TodoMappers
    {
        public static TodoDto ToTodoDto(this Todo Todo)
        {
            return new TodoDto
            {
                Title = Todo.Title,
                Description = Todo.Description,
                IsComplete = Todo.IsComplete,
                CreatedOn = Todo.CreatedOn,
                UpdatedOn = Todo.UpdatedOn
            };
        }


        public static Todo TodoRequestToTodo(this CreateTodoRequestDto TodoDto)
        {
            return new Todo
            {
                Title = TodoDto.Title,
                Description = TodoDto.Description,
                // IsComplete = TodoDto.IsComplete,
                // CreatedOn = TodoDto.CreatedOn,
                // UpdatedOn = TodoDto.UpdatedOn
            };
        }


        public static Todo TodoUpdateMarkCompleted(this MarkCompleteTodo TodoModel)
        {
            return new Todo
            {
                IsComplete = TodoModel.IsComplete,
                UpdatedOn = TodoModel.UpdatedOn
            };
        }

        public static Todo UpdateTodoRequestToTodo(this UpdateTodoRequestDto TodoModel){
            return new Todo {
                Title = TodoModel.Title,
                Description = TodoModel.Description,
                IsComplete = TodoModel.IsComplete,
                CreatedOn = TodoModel.CreatedOn,
                UpdatedOn = TodoModel.UpdatedOn
            };
        }


    }
}