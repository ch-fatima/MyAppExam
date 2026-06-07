using System;
using System.ComponentModel.DataAnnotations.Schema;
using wallet.lib.Base.Domain.Models;

namespace Core.Models.Entities
{
    [Table("Auth")]
    public class Auth : BaseEntity<Guid>
    {
        public string PhoneNo { get; set; }
        public Guid UserId { get; set; }
        public int? LoginCode { get; set; }
        public int LoginCodeTryCount { get; set; } = 0;
        public DateTime? LoginCodeExpirationDate { get; set; }
        public string LoginCodeSpecifiedValue { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
    }
}
