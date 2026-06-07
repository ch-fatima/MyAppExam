using Core.Models.Entities;
using Infrastructure.Repositories.IRepository;
using System;
using wallet.lib.dapper;

namespace Infrastructure.Repositories.Repository
{
    public class RoleRepository(DbSession session) : DapperRepository<Role, Guid>(session), IRoleRepository
    {
    }
}
