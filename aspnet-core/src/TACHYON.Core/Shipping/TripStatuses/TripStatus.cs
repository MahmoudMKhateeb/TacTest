using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Shipping.TripStatuses
{
    [Table("TripStatuses")]
    public class TripStatus : FullAuditedEntity
    {

        [Required]
        public virtual string DisplayName { get; set; }

    }
}