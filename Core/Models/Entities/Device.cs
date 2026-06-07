using Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using wallet.lib.Base.Domain.Models;

namespace Core.Models.Entities
{
    [Table("Device")]
    public class Device : BaseEntity<Guid>
    {
        public Guid UserId { get; set; }
        public string FirebaseToken { get; set; }
        public EPlatform Platform { get; set; }
        public EChannel? Channel { get; set; }
        public EMedia? Media { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpirationDate { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.Now;

    }
}
