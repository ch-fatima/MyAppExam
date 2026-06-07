using Core.Models.Dtos;
using System.Collections.Generic;

namespace Application.User.Dto
{
    public class LoginDto
    {
        public TokenDto Tokens { get; set; }
        public Core.Models.Entities.User User { get; set; }
        public List<Core.Models.Entities.Role> Roles { get; set; }
    }
}
