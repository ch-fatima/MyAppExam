using Core.Models.Dtos;
using Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using wallet.lib.dapper;

namespace Infrastructure.Repositories.IRepository
{
    public interface IUserClaimRepository : IRepository<UserClaim, Guid>
    {
        Task<List<UserClaimDto>> GetCliamsByUserId(Guid userId);
    }
}
