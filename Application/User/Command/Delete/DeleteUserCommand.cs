using MediatR;
using System;
using System.ComponentModel.DataAnnotations;

namespace Application.User.Command.Delete
{
    public class DeleteUserCommand : IRequest<bool>
    {
        [Required]
        public Guid Id { get; set; }
    }
}
