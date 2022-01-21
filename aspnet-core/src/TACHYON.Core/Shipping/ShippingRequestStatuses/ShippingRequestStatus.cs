using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Shipping.ShippingRequestStatuses
{
    [Table("ShippingRequestStatuses")]
    public class ShippingRequestStatus : FullAuditedEntity
    {
        [Required]
        [StringLength(ShippingRequestStatusConsts.MaxDisplayNameLength,
            MinimumLength = ShippingRequestStatusConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }
    }
}