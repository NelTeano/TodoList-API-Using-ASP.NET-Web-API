using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_API.Models;
using Web_API.DTO.Todo;

namespace Web_API.Mappers
{
    public static class TodoMappers
    {
        public static Todo ToTodoDto(this Todo TodoModel)
        {
            return new Todo
            {
                Id = TodoModel.Id,
                Title = TodoModel.Title,
                Description = TodoModel.Description,
                IsComplete = TodoModel.IsComplete,
                CreatedOn = TodoModel.CreatedOn,
                UpdatedOn = TodoModel.UpdatedOn
            };
        }


        public static Todo CreateTodoRequestToTodo(this CreateTodoRequestDto TodoModel)
        {
            return new Todo
            {
                Title = TodoModel.Title,
                Description = TodoModel.Description,
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