using Core.Models.Entities;
using System;
using wallet.lib.dapper;

namespace Infrastructure.Repositories.IRepository
{
    public interface IRoleRepository : IRepository<Role, Guid>
    {
    }
}
