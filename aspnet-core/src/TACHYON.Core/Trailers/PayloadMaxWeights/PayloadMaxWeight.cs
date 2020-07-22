using Abp.Auditing;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.Trailers.PayloadMaxWeights
{
    [Table("PayloadMaxWeights")]
    [Audited]
    public class PayloadMaxWeight : FullAuditedEntity
    {

        [Required]
        [StringLength(PayloadMaxWeightConsts.MaxDisplayNameLength, MinimumLength = PayloadMaxWeightConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        public virtual int MaxWeight { get; set; }


    }
}