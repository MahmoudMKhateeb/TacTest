using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.Goods
{
    [Table("DangerousGoodTypeTranslations")]
    public class DangerousGoodTypeTranslation : FullAuditedEntity, IEntityTranslation<DangerousGoodType>
    {
        [Required]
        [StringLength(DangerousGoodTypeConsts.MaxNameLength, MinimumLength = DangerousGoodTypeConsts.MinNameLength)]
        public virtual string TranslatedName { get; set; }
        public string Language { get; set; }
        public DangerousGoodType Core { get; set; }
        public int CoreId { get; set; }
    }
}