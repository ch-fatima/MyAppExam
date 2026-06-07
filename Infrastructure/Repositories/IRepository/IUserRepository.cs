using Core.Models.Dtos;
using Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using wallet.lib.dapper;

namespace Infrastructure.Repositories.IRepository
{
    public interface IUserRepository : IRepository<User, Guid>
    {
        Task<User> GetUserByUserName(string EmailOrPhone);
        Task<User> GetUserByUserNameWithoutParameter(string UserName);
        Task<List<UserDto>> GetUsers(GetUserDto user);
        Task<bool> IsExistUser(User user);
    }
}
