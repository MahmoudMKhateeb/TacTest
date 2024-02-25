using TACHYON.Redemption;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Redemption
{
    [Table("RedemptionCodes")]
    public class RedemptionCode : FullAuditedEntity<long>
    {

        public virtual DateTime RedemptionDate { get; set; }

        public virtual long RedemptionTenantId { get; set; }

        public virtual long? RedeemCodeId { get; set; }

        [ForeignKey("RedeemCodeId")]
        public RedeemCode RedeemCodeFk { get; set; }

    }
}