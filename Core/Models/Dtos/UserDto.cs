using System;

namespace Core.Models.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string NationalCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public DateTime CreateDate { get; set; } 
        public bool IsActive { get; set; } 
    }

}
