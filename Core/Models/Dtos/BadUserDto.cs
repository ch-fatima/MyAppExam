using System;

namespace Core.Models.Dtos
{
    public class BadUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Guid? RoleId { get; set; }
        public string PhoneNo { get; set; }
    }
}
