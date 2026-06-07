using Core.Exceptions;
using Core.Extensions;
using Core.Models;
using Core.Models.Dtos;
using Core.Models.Entities;
using Infrastructure.Cryptography;
using Infrastructure.Logger;
using Infrastructure.UnitOfWork;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace Application.User.Command.Create
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, PUserDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICryptographyService _cryptoService;
        private readonly ILoggerManager _loggerManager;
        private readonly InitSetting _initSetting;

        public RegisterUserCommandHandler(IUnitOfWork unitOfWork, ILoggerManager loggerManager, InitSetting initSetting, ICryptographyService cryptoService)
        {
            _unitOfWork = unitOfWork;
            _loggerManager = loggerManager;
            _initSetting = initSetting;
            _cryptoService = cryptoService;
        }

        public async Task<PUserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = new Core.Models.Entities.User()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.UserName,
                PhoneNo = request.PhoneNo
            };
            var userExist = await _unitOfWork.UserRepository.IsExistUser(user);
            _loggerManager.AddMiddleLog("userExist Request=>", JsonConvert.SerializeObject(userExist));
            if (userExist)
            {
                _loggerManager.LogError($"UserExist => Email = {request.Email}, UserName={request.UserName}, PhoneNo={request.PhoneNo}");
                throw new BadRequestException("کاربر وارد شده تکراری است.");
            }
            try
            {
                _unitOfWork.BeginTransaction();
                string encryptedNationalCode = _cryptoService.Encrypt(request.NationalCode);
                user.NationalCode = encryptedNationalCode;
                await _unitOfWork.UserRepository.InsertAsync(user);
                _loggerManager.LogInfo($"Insert User Done UserName={request.UserName}");
                #region--Register Auth--
                var sault = request.Password == null || string.IsNullOrEmpty(request.Password) ? null : SecurityHelper.GenerateSalt();
                var passwordHash = request.Password == null || string.IsNullOrEmpty(request.Password) ? null : SecurityHelper.HashPassword(request.Password, sault);

                var newCodeValue = SecurityHelper.GenerateRandomCode();
                Auth auth = new()
                {
                    LoginCode = newCodeValue,
                    PhoneNo = request.PhoneNo,
                    UserId = user.Id,
                    LoginCodeTryCount = 0,
                    Password = request.Password == null || string.IsNullOrEmpty(request.Password) ? null : passwordHash,
                    Salt = request.Password == null || string.IsNullOrEmpty(request.Password) ? null : sault,
                    LoginCodeExpirationDate = DateTime.Now.AddMinutes(1)
                };
                await _unitOfWork.AuthRepository.InsertAsync(auth);
                _loggerManager.LogInfo($"Auth User Done UserName={request.UserName}");
                #endregion
                #region create user role
                UserRole userRole = new()
                {
                    UserId = user.Id,
                    RoleId = request.RoleId ?? Guid.Parse(_initSetting.DefaultuserRoleId)
                };
                await _unitOfWork.UserRoleRepository.InsertAsync(userRole);
                _loggerManager.LogInfo($"UserRole User Done UserName={request.UserName}");
                #endregion
                _unitOfWork.Commit();
            }
            catch (AppException ex)
            {
                _loggerManager.LogError($" {ex} Resister User Error UserName={request.UserName}");
                _unitOfWork.Rollback();
                throw new BadRequestException("ثبت کاربر با خطا مواجه شد.");
            }
            _loggerManager.LogInfo($"Register User Done UserName={request.UserName}");
            return new PUserDto()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName
            };
        }
    }
}
