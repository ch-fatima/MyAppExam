using Core.ApiResults;
using Core.Exceptions;
using Infrastructure.Logger;
using Infrastructure.TokenHelper;
using Infrastructure.UnitOfWork;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.User.Command.Delete
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoggerManager _logger;
        private readonly ICurrentUserHelper _currentUserHelper;

        public DeleteUserCommandHandler(IUnitOfWork unitOfWork, ILoggerManager logger, ICurrentUserHelper currentUserHelper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _currentUserHelper = currentUserHelper;
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var userToken = _currentUserHelper.GetUserFromToken();
            if (userToken is null)
                throw new AppException(ApiResultStatusCode.UnAuthorized);

            var user = await _unitOfWork.UserRepository.GetByIdAsync(request.Id);
            if (user is null)
                throw new NotFoundException("کاربر یافت نشد");

            var delete = await _unitOfWork.UserRepository.DeleteAsync(user);
            if (!delete)
                throw new BadRequestException("خطا در حذف کاربر");

            _logger.LogInfo($"Delete User = {user.UserName} BY UserAdmin = {userToken.UserName}");
            return delete;
        }
    }
}
