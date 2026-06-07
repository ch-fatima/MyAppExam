using Application.User.Dto;
using Core.Exceptions;
using Core.Models;
using Core.Models.Dtos;
using Core.Models.Entities;
using Infrastructure.Logger;
using Infrastructure.TokenHelper;
using Infrastructure.UnitOfWork;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Application.User.Command.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenGenerateHelper _tokenHelper;
        private readonly ILoggerManager _logger;
        private readonly InitSetting _setting;

        public RefreshTokenCommandHandler(IUnitOfWork unitOfWork, ITokenGenerateHelper tokenHelper, InitSetting setting, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _tokenHelper = tokenHelper;
            _setting = setting;
            _logger = logger;
        }

        public async Task<LoginDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var device = await _unitOfWork.DeviceRepository.GetDeviceByRefreshToken(request.RefreshToken);
            if ( device is null)
            {
                throw new NotFoundException("توکن یافت نشد.");
            }
            if (DateTime.Now > device.RefreshTokenExpirationDate)
            {
                throw new BadRequestException("توکن منقضی شده است.");
            }

            if (device.RefreshToken != request.RefreshToken || !device.IsActive)
            {
                throw new BadRequestException("توکن نامعتبر است.");
            }
            var user = await _unitOfWork.UserRepository.GetByIdAsync(device.UserId);
            var userClaims = await _unitOfWork.UserClaimRepository.GetCliamsByUserId(user.Id);

            TokenDto tokens = _tokenHelper.GenerateTokens(user, device.Id, Guid.Parse(_setting.BusinessKey), userClaims, null);
            
            await _unitOfWork.DeviceRepository.UpdateDeviceRefreshToken(device.Id, tokens.RefreshToken, tokens.RefreshTokenExpirationDate);
            _logger.LogInfo($"RefreshToken Success By UserName= {user.UserName}");
            return new LoginDto { Tokens=tokens, User=user};
        }
    }
}
