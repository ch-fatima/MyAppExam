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
    public class UserRoleRepository(DbSession session): DapperRepository<UserRole, Guid>(session), IUserRoleRepository
    {
        public async Task<List<Role>> GetUserRoles(Guid userId)
        {
            string query = "select * from UserRole where UserId=@UserId";
            DynamicParameters parameters = new();
            parameters.Add("@UserId", userId);
            var roles = await Session.Connection.QueryAsync<Role>(query, 
                param: parameters, commandType: CommandType.Text, transaction: Session.Transaction);
            return roles.ToList();
        }

        public async Task<List<string>> GetRoleNameById(Guid UserId)
        {
            DynamicParameters parameters = new();
            parameters.Add("@UserId", UserId);
            var lstRoleName = await Session.Connection
              .QueryAsync<string>("select distinct r.Name  from [dbo].[Role] r  " +
              "inner join [dbo].[UserRole] ur on r.Id = ur.RoleId  WHERE UserId=@UserId",
          parameters, transaction: base.Session.Transaction);
            return lstRoleName.ToList<string>();
        }
    }
}
