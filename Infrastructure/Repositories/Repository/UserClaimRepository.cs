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
    public class UserClaimRepository(DbSession session) : DapperRepository<UserClaim, Guid>(session), IUserClaimRepository
    {
        public async Task<List<UserClaimDto>> GetCliamsByUserId(Guid userId)
        {
            string query = "[dbo].[UserClaim_GetClaims]";
            DynamicParameters parameters = new();
            parameters.Add("@UserId", userId);
            var userClaims = await Session.Connection.QueryAsync<UserClaimDto>(sql: query,
                param: parameters, commandType: CommandType.StoredProcedure,
                transaction: Session.Transaction);
            return userClaims.ToList();
        }
    }
}
