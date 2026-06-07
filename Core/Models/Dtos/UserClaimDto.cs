using Newtonsoft.Json;
using System;
using wallet.lib.Base.Domain;

namespace Core.Models.Dtos
{
    public class UserClaimDto : IEntity<Guid?>
    {
        [JsonIgnore]
        public Guid? Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? ClaimId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
