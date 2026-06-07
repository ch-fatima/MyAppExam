using Core.Models.Entities;
using Infrastructure.Repositories.IRepository;
using System;
using wallet.lib.dapper;

namespace Infrastructure.Repositories.Repository
{
    public class ClaimRepository(DbSession session) : DapperRepository<Claim, Guid>(session), IClaimRepository
    {
    }
}
