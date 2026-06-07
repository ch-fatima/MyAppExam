using System;
using System.ComponentModel.DataAnnotations.Schema;
using wallet.lib.Base.Domain.Models;

namespace Core.Models.Entities
{
    [Table("UserClaim")]

    public class UserClaim : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public Guid ClaimId { get; set; }
        public string Value { get; set; }
    }
}
