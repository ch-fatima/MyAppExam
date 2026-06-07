using Core.Models.Dtos;
using Core.Models.Entities;
using System;
using System.Threading.Tasks;
using wallet.lib.dapper;

namespace Infrastructure.Repositories.IRepository
{
    public interface IDeviceRepository : IRepository<Device, Guid>
    {
        Task<Device> CreateNewDeviceByToken(Guid deviceId, TokenDto tokens, Guid userId);
        Task<Device> GetDeviceByRefreshToken(string refreshToken);
        Task<bool> RevokeAllRefreshToken(Guid userId);
        Task<bool> UpdateDeviceRefreshToken(Guid? deviceId, string refreshToken, DateTime? RefreshTokenExpirationDate);
    }
}
