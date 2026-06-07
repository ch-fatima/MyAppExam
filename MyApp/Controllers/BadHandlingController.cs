using Core.Extensions;
using Core.Models.Dtos;
using Core.Models.Entities;
using Infrastructure.Logger;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BadHandlingController : ControllerBase
    {
        ///User Working With DB
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILoggerManager _loggerManager;

        public BadHandlingController(IUnitOfWork unitOfWork, ILoggerManager loggerManager)
        {
            _unitOfWork = unitOfWork;
            _loggerManager = loggerManager;
        }

        /// <summary>
        /// Bad Handling
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost(template: "BadUserRegister")]
        [ProducesResponseType(type: typeof(ActionResult), statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(type: typeof(ActionResult), statusCode: StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BadUserRegisterAsync([FromBody] BadUserDto request)
        {
            //بدترین حالت میتونست از entity به عنوان پارامتر ورودی استفاده کنه 
            var user = new User()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.UserName,
                PhoneNo = request.PhoneNo
            };
            //به جای اعتبارسنجی وجود داشتن کاربر اطلاعات کاربر را برمی گرداند
            //WithoutParameter
            var userExist = await _unitOfWork.UserRepository.GetUserByUserNameWithoutParameter(request.UserName);
            _loggerManager.AddMiddleLog("userExist Request=>", JsonConvert.SerializeObject(userExist));
            if (userExist is not null)
            {
                _loggerManager.LogError($"UserExist => Email = {request.Email}, UserName={request.UserName}, PhoneNo={request.PhoneNo}");
                //return UserId
                return BadRequest($"User already exists. UserId: {userExist.Id}");
            }
            try
            {
                _unitOfWork.BeginTransaction();
                var newUser = await _unitOfWork.UserRepository.InsertAsync(user);
                _loggerManager.LogInfo("Insert User Done");
                #region--Register Auth--
                var newCodeValue = SecurityHelper.GenerateRandomCode();
                Auth auth = new()
                {
                    LoginCode = newCodeValue,
                    PhoneNo = request.PhoneNo,
                    UserId = user.Id,
                    LoginCodeTryCount = 0,
                    ///Dont Hash Password
                    Password = request.Password,
                    Salt = request.Password,
                    LoginCodeExpirationDate = DateTime.Now.AddMinutes(1)
                };
                await _unitOfWork.AuthRepository.InsertAsync(auth);
                #endregion
                #region create user role
                UserRole userRole = new()
                {
                    UserId = user.Id,
                    RoleId = request.RoleId.Value
                };
                await _unitOfWork.UserRoleRepository.InsertAsync(userRole);
                #endregion
                _unitOfWork.Commit();
                _loggerManager.LogInfo("Register User Done");
                //Return New User Info From Entity
                return Ok(newUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.ToString(),  //  stack trace
                    innerException = ex.InnerException?.ToString(),// inner exception
                    source = ex.Source,
                    data = ex.Data // Has Sensitive data
                });
            }
        }
    }
}
