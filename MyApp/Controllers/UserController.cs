using Application.User.Command.Create;
using Application.User.Command.Delete;
using Application.User.Command.Login;
using Application.User.Command.LogOut;
using Application.User.Command.RefreshToken;
using Application.User.Dto;
using Application.User.Query.GetUser;
using Core.ApiResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiApp.Controllers
{
    public class UserController : BaseController
    {
        public UserController(IMediator mediator) : base(mediator)
        {
        }

        /// <summary>
        /// Good Handling
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost(template: "GoodRegister")]
        [ProducesResponseType(type: typeof(ActionResult), statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(type: typeof(ActionResult), statusCode: StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Core.Models.Dtos.PUserDto>> CreateUserAsync([FromBody] RegisterUserCommand request)
        {
            Core.Models.Dtos.PUserDto result = await Mediator.Send(request);
            if (result != null)
                return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        /// GetUserList
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet(template: "Get")]
        [ProducesResponseType(type: typeof(ActionResult<List<Core.Models.Dtos.UserDto>>), statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(type: typeof(ActionResult), statusCode: StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<Core.Models.Dtos.UserDto>>> GetUsersAsync([FromQuery]GetUserQuery request)
        {
            List<Core.Models.Dtos.UserDto> result = await Mediator.Send(request);
            return Ok(result);
        }

        /// <summary>
        /// Delete User By Admin
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete(template: "Delete")]
        [ProducesResponseType(type: typeof(ActionResult<bool>), statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(type: typeof(ActionResult), statusCode: StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> DeleteUserAsync(DeleteUserCommand request)
        {
            return Ok(await Mediator.Send(request));
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost(template: "Login")]
        [ProducesResponseType(type: typeof(ActionResult<LoginDto>), statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(type: typeof(ActionResult), statusCode: StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LoginDto>> LoginUserAsync([FromBody] LoginCommand request)
        {
            return Ok(await Mediator.Send(request));
        }

        /// <summary>
        /// Refresh
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost(template: "Refresh")]
        [ProducesResponseType(type: typeof(ActionResult<LoginDto>), statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(type: typeof(ActionResult), statusCode: StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LoginDto>> RefreshAsync([FromBody] RefreshTokenCommand request)
        {
            return Ok(await Mediator.Send(request));
        }

        /// <summary>
        /// LogOut
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost(template: "LogOut")]
        [ProducesResponseType(type: typeof(ActionResult<bool>), statusCode: StatusCodes.Status200OK)]
        [ProducesResponseType(type: typeof(ActionResult), statusCode: StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LoginDto>> LogOutUserAsync()
        {
            return Ok(await Mediator.Send(new LogOutCommand()));
        }
    }
}
