using Infrastructure.Cryptography;
using Infrastructure.Logger;
using Infrastructure.Repositories.IRepository;
using Infrastructure.Repositories.Repository;
using Infrastructure.TokenHelper;
using Microsoft.Extensions.DependencyInjection;
using wallet.lib.dapper;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services)
        {
            services.AddScoped<DbSession>();
            services.AddTransient<UnitOfWork.IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddTransient
                <Microsoft.AspNetCore.Http.IHttpContextAccessor,
                Microsoft.AspNetCore.Http.HttpContextAccessor>();
            services.AddTransient<ILoggerManager, LoggerManager>();
            services.AddScoped<ITokenGenerateHelper, TokenGenerateHelper>();
            services.AddScoped<ICurrentUserHelper, CurrentUserHelper>();
            services.AddScoped<ITokenBlacklistHelper, TokenBlacklistHelper>();
            services.AddScoped<ICryptographyService, AesGcmService>();
            #region--Repositories DI--
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IAuthRepository, AuthRepository>();
            services.AddTransient<IRoleRepository, RoleRepository>();
            services.AddTransient<IUserRoleRepository, UserRoleRepository>(); 
            services.AddTransient<IClaimRepository, ClaimRepository>();
            services.AddTransient<IUserClaimRepository, UserClaimRepository>();
            services.AddTransient<IDeviceRepository, DeviceRepository>();
            #endregion
            return services;    
        }
    }
}
