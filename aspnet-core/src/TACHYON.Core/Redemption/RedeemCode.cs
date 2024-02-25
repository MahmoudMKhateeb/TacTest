using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace TACHYON.Redemption
{
    [Table("RedeemCodes")]
    public class RedeemCode : FullAuditedEntity<long>
    {

        [StringLength(RedeemCodeConsts.MaxCodeLength, MinimumLength = RedeemCodeConsts.MinCodeLength)]
        public virtual string Code { get; set; }

        public virtual DateTime? ExpiryDate { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual decimal Value { get; set; }

        public virtual string Note { get; set; }

        [Range(RedeemCodeConsts.MinpercentageValue, RedeemCodeConsts.MaxpercentageValue)]
        public virtual int percentage { get; set; }

    }
}