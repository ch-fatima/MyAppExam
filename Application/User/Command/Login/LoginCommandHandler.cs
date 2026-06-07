using Application.User.Dto;
using Core.Exceptions;
using Core.Extensions;
using Core.Models;
using Core.Models.Dtos;
using Infrastructure.Logger;
using Infrastructure.TokenHelper;
using Infrastructure.UnitOfWork;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.User.Command.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly InitSetting _setting;
        private readonly ITokenGenerateHelper _tokenHelper;
        private readonly ILoggerManager _logger;

        public LoginCommandHandler(IUnitOfWork unitOfWork, InitSetting setting, ITokenGenerateHelper tokenHelper, ILoggerManager logger)
        {
            _unitOfWork = unitOfWork;
            _setting = setting;
            _tokenHelper = tokenHelper;
            _logger = logger;
        }

        public async Task<LoginDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUserName(request.UserName);
            if (user == null)
            {
                throw new NotFoundException("کاربر یافت نشد.");
            }

            var authUser = await _unitOfWork.AuthRepository.GetUserAuth(user.Id);
            if (authUser == null)
            {
                throw new NotFoundException("احراز هویت کاربر یافت نشد.");
            }

            if (!SecurityHelper.VerifyPassword(password: request.Password, salt: authUser.Salt, hash: authUser.Password))
            {
                throw new BadRequestException("عدم تطابق کلمه عبور");
            }
            var roles = await _unitOfWork.UserRoleRepository.GetUserRoles(user.Id);
            if (!roles.Any())
                throw new NotFoundException("نقشی برای کاربر یافت نشد");

            var UserRoleName = await _unitOfWork.UserRoleRepository.GetRoleNameById(user.Id);
            var userClaims = await _unitOfWork.UserClaimRepository.GetCliamsByUserId(user.Id);
            var deviceId = Guid.NewGuid();          

            TokenDto tokens = _tokenHelper.GenerateTokens(user, deviceId, Guid.Parse(_setting.BusinessKey), userClaims, UserRoleName);
            await _unitOfWork.DeviceRepository.CreateNewDeviceByToken(deviceId, tokens, user.Id);
            var userRoles = await _unitOfWork.UserRoleRepository.GetUserRoles(user.Id);
            if (!userRoles.Any())
                throw new BadRequestException("عدم وجود نقش های کاربر");

            _logger.LogInfo($"User Login Success By UserName= {request.UserName}");
            return new LoginDto() { Tokens = tokens, User = user, Roles = userRoles };
        }
    }
}
