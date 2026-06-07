using Core.Models.Dtos;
using Infrastructure.Cryptography;
using Infrastructure.UnitOfWork;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.User.Query.GetUser
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, List<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICryptographyService _cryptoService;

        public GetUserQueryHandler(IUnitOfWork unitOfWork, ICryptographyService cryptoService)
        {
            _unitOfWork = unitOfWork;
            _cryptoService = cryptoService;
        }

        public async Task<List<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var userDto = new GetUserDto()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNo = request.PhoneNo,
                UserName = request.UserName,
                IsActive = request.IsActive,
            };
            var userList = await _unitOfWork.UserRepository.GetUsers(userDto);
            return userList;
        }
    }
}
