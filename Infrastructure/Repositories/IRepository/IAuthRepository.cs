using Core.Models.Entities;
using System;
using System.Threading.Tasks;
using wallet.lib.dapper;

namespace Infrastructure.Repositories.IRepository
{
    public interface IAuthRepository : IRepository<Auth, Guid>
    {
        Task<Auth> GetUserAuth(Guid userId);
    }
}
