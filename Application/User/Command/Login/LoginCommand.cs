using Application.User.Dto;
using Core.Attribute;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.User.Command.Login
{
    public class LoginCommand : IRequest<LoginDto>
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "UserName")]
        [SafeInputValidation]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
