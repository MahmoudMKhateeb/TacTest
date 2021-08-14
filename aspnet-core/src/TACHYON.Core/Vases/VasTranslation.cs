using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TACHYON.Vases
{
    [Table("VasesTranslations")]
    public class VasTranslation : FullAuditedEntity,IEntityTranslation<Vas>
    {
        [StringLength(VasConsts.MaxDisplayNameLength, MinimumLength = VasConsts.MinDisplayNameLength)]
        public virtual string DisplayName { get; set; }
        public string Language { get; set; }
        public Vas Core { get; set; }
        public int CoreId { get; set; }
    }
}
