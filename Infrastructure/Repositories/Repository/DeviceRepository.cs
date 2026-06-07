using Core.Models.Dtos;
using Core.Models.Entities;
using Dapper;
using Infrastructure.Repositories.IRepository;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using wallet.lib.dapper;

namespace Infrastructure.Repositories.Repository
{
    public class DeviceRepository(DbSession session) : DapperRepository<Device, Guid>(session), IDeviceRepository
    {
        public async Task<Device> CreateNewDeviceByToken(Guid deviceId, TokenDto tokens, Guid userId)
        {            

            var device = new Device()
            {
                RefreshToken = tokens.RefreshToken,
                RefreshTokenExpirationDate = tokens.RefreshTokenExpirationDate,
                UserId = userId,
                CreateAt = DateTime.Now,
                //Channel = channel,
                //Media = media,
                // Id = deviceId,
                IsActive = true
            };
            device.Id = deviceId;
            var insertedDevice = await InsertAsync(device);
            return insertedDevice;
        }

        public async Task<Device> GetDeviceByRefreshToken(string refreshToken)
        {
            DynamicParameters parameters = new();
            parameters.Add("RefreshToken", refreshToken);
            string query = $"WITH (NOLOCK) WHERE RefreshToken=@RefreshToken";
            var result = await QueryAsync(query, parameters);
            var device = result.FirstOrDefault();
            return device;
        }

        public async Task<bool> UpdateDeviceRefreshToken(Guid? deviceId, string refreshToken, DateTime? RefreshTokenExpirationDate)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@DeviceId", deviceId);
            parameters.Add("@RefreshToken", refreshToken);
            parameters.Add("@RefreshTokenExpirationDate", RefreshTokenExpirationDate);
            var updateDevice = await Session.Connection.QueryAsync<bool>(
            sql: "[dbo].[Device_UpdateRefreshToken]", param: parameters, commandType: CommandType.StoredProcedure, transaction: Session.Transaction);
            return true;
        }

        public async Task<bool> RevokeAllRefreshToken(Guid userId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            var updateDevice = await Session.Connection.QueryAsync<bool>(
            sql: "[dbo].[Device_RevokeRefreshToken]", param: parameters, commandType: CommandType.StoredProcedure, transaction: Session.Transaction);
            return true;
        }

    }
}
