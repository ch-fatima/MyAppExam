using Core.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using wallet.lib.Base.Domain.Models;

namespace Core.Models.Entities
{
    [Table("User")]
    public class User : BaseEntity<Guid>
    {
        public string NationalCode { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public string PhoneNo { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;  
    }
}
