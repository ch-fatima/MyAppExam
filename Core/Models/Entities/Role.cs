using System;
using System.ComponentModel.DataAnnotations.Schema;
using wallet.lib.Base.Domain.Models;

namespace Core.Models.Entities
{
    [Table("Role")]
    public class Role : BaseEntity<Guid>
    {
        public string Name { get; set; }

        public string PName { get; set; }

        public int Priority { get; set; }

        public string Description { get; set; }

        public bool IsAdmin { get; set; }

    }
}
