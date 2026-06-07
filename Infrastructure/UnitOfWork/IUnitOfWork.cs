using Infrastructure.Repositories.IRepository;
using System.Data;

namespace Infrastructure.UnitOfWork
{
    public interface IUnitOfWork : wallet.lib.dapper.IUnitOfWork
    {
        public IDbConnection Connection { get; }
        public IUserRepository UserRepository { get; }
        public IAuthRepository AuthRepository { get; }
        public IDeviceRepository DeviceRepository { get; }
        public IClaimRepository ClaimRepository { get; }
        public IRoleRepository RoleRepository { get; }
        public IUserClaimRepository UserClaimRepository { get; }
        public IUserRoleRepository UserRoleRepository { get; }
    }
}
