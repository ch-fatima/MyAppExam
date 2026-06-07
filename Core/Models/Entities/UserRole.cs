using System;
using System.ComponentModel.DataAnnotations.Schema;
using wallet.lib.Base.Domain.Models;

namespace Core.Models.Entities
{
    [Table("UserRole")]
    public class UserRole : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }

        public Guid RoleId { get; set; }
    }
}
