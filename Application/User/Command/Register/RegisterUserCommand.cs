using Application.User.Dto;
using Core.Models.Dtos;
using MediatR;

namespace Application.User.Command.Create
{
    public class RegisterUserCommand : Dto.UserDto, IRequest<PUserDto> 
    {
    }    
}
