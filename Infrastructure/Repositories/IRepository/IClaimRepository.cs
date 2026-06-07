using Core.Models.Entities;
using System;
using wallet.lib.dapper;

namespace Infrastructure.Repositories.IRepository
{
    public interface IClaimRepository : IRepository<Claim, Guid>
    {
    }
}
