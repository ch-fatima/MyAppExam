using Core.Models.Entities;
using Dapper;
using Infrastructure.Repositories.IRepository;
using System;
using System.Linq;
using System.Threading.Tasks;
using wallet.lib.dapper;

namespace Infrastructure.Repositories.Repository
{
    public class AuthRepository(DbSession session):DapperRepository<Auth, Guid>(session), IAuthRepository
    {
        public async Task<Auth> GetUserAuth(Guid userId)
        {
            DynamicParameters parameters = new();
            parameters.Add("UserId", userId);

            string query = $"WITH (NOLOCK) WHERE UserId=@UserId";
            var result = await QueryAsync(query, parameters);

            return result.FirstOrDefault();
        }
    }
}
