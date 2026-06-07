using Application.User.Dto;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.User.Command.RefreshToken
{
    public class RefreshTokenCommand : IRequest<LoginDto>
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
