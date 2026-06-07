using System;
using System.ComponentModel.DataAnnotations.Schema;
using wallet.lib.Base.Domain.Models;

namespace Core.Models.Entities
{
    [Table("Claim")]
    public class Claim : BaseEntity<Guid>
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
