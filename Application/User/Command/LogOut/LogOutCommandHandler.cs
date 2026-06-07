using Core.ApiResults;
using Core.Exceptions;
using Core.Extensions;
using FluentResults;
using Infrastructure.Logger;
using Infrastructure.TokenHelper;
using Infrastructure.UnitOfWork;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Application.User.Command.LogOut
{
    public class LogOutCommandHandler : IRequestHandler<LogOutCommand, Result<bool>>
    {
        private readonly ICurrentUserHelper _currentUserHelper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoggerManager _logger;
        private readonly ITokenBlacklistHelper _blacklistHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogOutCommandHandler(IUnitOfWork unitOfWork, ICurrentUserHelper currentUserHelper, ILoggerManager logger, ITokenBlacklistHelper blacklistHelper, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _currentUserHelper = currentUserHelper;
            _logger = logger;
            _blacklistHelper = blacklistHelper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<bool>> Handle(LogOutCommand request, CancellationToken cancellationToken)
        {
            var userToken = _currentUserHelper.GetUserFromToken();
            if(userToken is null)
                throw new AppException(ApiResultStatusCode.UnAuthorized);

            var user = await _unitOfWork.UserRepository.GetUserByUserName(userToken.UserName);
            if (user == null)
            {
                _logger.LogWarn($"User not found for UserName: {userToken.UserName}");
                throw new NotFoundException("کاربر یافت نشد.");
            }

            var authUser = await _unitOfWork.AuthRepository.GetUserAuth(user.Id);
            if (authUser == null)
            {
                _logger.LogWarn($"Auth not found for UserId: {user.Id}");
                throw new NotFoundException("احراز هویت کاربر یافت نشد.");
            }

            var accessToken = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                .ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(accessToken))
            {
                // استخراج زمان انقضای توکن
                var expiry = SecurityHelper.GetTokenExpiry(accessToken);

                // اضافه کردن به Blacklist (فوری غیرفعال میشه)
                await _blacklistHelper.AddToBlacklistAsync(accessToken, expiry);

                _logger.LogInfo($"Access token blacklisted for user {user.Id}, expires at {expiry}");
            }

            var isRevoke = await _unitOfWork.DeviceRepository.RevokeAllRefreshToken (authUser.UserId);
            if(!isRevoke)
                throw new BadRequestException("خروج کاربر با خطا مواجه شد");

            _logger.LogInfo($"UserLogOut Success By UserName= {user.UserName}");
            return true;
        }
        
    }
}
