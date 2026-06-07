using Core.Models.Dtos;
using Core.Models.Entities;
using Dapper;
using Infrastructure.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using wallet.lib.dapper;

namespace Infrastructure.Repositories.Repository
{
    public class UserRepository(DbSession session) 
        : DapperRepository<User, Guid>(session),IUserRepository
    {
        public async Task<bool> IsExistUser(User user)
        { 
            DynamicParameters parameters= new DynamicParameters();
            parameters.Add("@UserName", user.UserName);
            parameters.Add("@Email", user.Email);
            parameters.Add("@PhoneNo", user.PhoneNo);
            var query = $"select * from dbo.[User] where UserName=@UserName and Email=@Email and PhoneNo=@PhoneNo";
            var result = await Session.Connection.QueryAsync(query, parameters, 
                transaction:Session.Transaction, commandType:CommandType.Text);
            if (result != null && result.Any())
            {
                return true;
            }
            return false;
        }

        public async Task<User> GetUserByUserName(string UserName)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserName", UserName);
            var query = $"select * from dbo.[User] where UserName=@UserName";
            var result = await Session.Connection.QueryFirstOrDefaultAsync<User>(query, parameters,
                transaction: Session.Transaction, commandType: CommandType.Text);
            return result;
        }

        public async Task<User> GetUserByUserNameWithoutParameter(string UserName)
        {
            var query = $"select * from dbo.[User] where UserName = '{UserName}'";
            var result = await Session.Connection.QueryFirstOrDefaultAsync<User>(query,
                transaction: Session.Transaction, commandType: CommandType.Text);
            return result;
        }

        public async Task<List<UserDto>> GetUsers(GetUserDto user)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserName", user.UserName);
            parameters.Add("@Email", user.Email);
            parameters.Add("@PhoneNo", user.PhoneNo);
            parameters.Add("@FirstName", user.FirstName);
            parameters.Add("@LastName", user.LastName);
            parameters.Add("@IsActive", user.IsActive);
            var spName = "[dbo].[User_Get]";
            var result = await Session.Connection.QueryAsync<UserDto>(spName, parameters,
                transaction: Session.Transaction, commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

    }
}
