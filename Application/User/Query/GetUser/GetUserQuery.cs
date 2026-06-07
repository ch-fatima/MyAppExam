using Core.Models.Dtos;
using MediatR;
using System.Collections.Generic;

namespace Application.User.Query.GetUser
{
    public class GetUserQuery : GetUserDto, IRequest<List<UserDto>>
    {
    }
}
