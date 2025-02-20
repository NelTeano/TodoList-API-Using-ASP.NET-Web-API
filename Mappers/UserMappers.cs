using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web_API.Models;
using Web_API.DTO.User;

namespace Web_API.Mappers
{
    public static class UserMappers
    {
        public static User ToUserDto(this UserDto UserModel)
        {
            return new User
            {
                Id = UserModel.Id,
                username = UserModel.username,
                password = UserModel.password
            };
        }

        public static User CreateUserRequest(this CreateUserDto UserModel)
        {
            return new User
            {
                username = UserModel.username,
                password = UserModel.password
            };

        }
    }
}