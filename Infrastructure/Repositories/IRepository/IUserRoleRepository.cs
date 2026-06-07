using Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using wallet.lib.dapper;

namespace Infrastructure.Repositories.IRepository
{
    public interface IUserRoleRepository : IRepository<UserRole, Guid>
    {
        Task<List<string>> GetRoleNameById(Guid UserId);
        Task<List<Role>> GetUserRoles(Guid userId);
    }
}
