using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Core.Models.Dtos
{
    public class UserTokenDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string PhoneNo { get; set; }

        public string Email { get; set; }

        public List<string> Roles { get; set; }

        public List<string> UserRoles { get; set; }

        public List<Claim> Claims { get; set; }
    }
}
