using Core.Models.Entities;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Net.WebSockets;

namespace Application.User.Command.LogOut
{
    public class LogOutCommand : IRequest<Result<bool>>
    {
    }
}
