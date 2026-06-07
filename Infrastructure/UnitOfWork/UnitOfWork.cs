using Infrastructure.Repositories.IRepository;
using System.Data;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfWork : wallet.lib.dapper.UnitOfWork, IUnitOfWork
    {
        public IDbConnection Connection { get; }

        public IUserRepository UserRepository { get; }
        public IAuthRepository AuthRepository { get; }
        public IDeviceRepository DeviceRepository { get; }
        public IClaimRepository ClaimRepository { get; }
        public IRoleRepository RoleRepository { get; }
        public IUserClaimRepository UserClaimRepository { get; }
        public IUserRoleRepository UserRoleRepository { get; }

        public UnitOfWork(wallet.lib.dapper.DbSession session,
            IDbConnection connection,
            IUserRepository userRepository,
            IAuthRepository authRepository,
            IDeviceRepository deviceRepository,
            IClaimRepository claimRepository,
            IRoleRepository roleRepository,
            IUserClaimRepository userClaimRepository,
            IUserRoleRepository userRoleRepository) : base(session)
        {
            Connection = connection;
            UserRepository = userRepository;
            AuthRepository = authRepository;
            DeviceRepository = deviceRepository;
            ClaimRepository = claimRepository;
            RoleRepository = roleRepository;
            UserClaimRepository = userClaimRepository;
            UserRoleRepository = userRoleRepository;
        }
    }
}
